<?php

use Slim\App;
use Slim\Http\Request;
use Slim\Http\Response;
use Slim\Http\UploadedFile;
use Sk\Geohash\Geohash;
use \Firebase\JWT\JWT;
use ReallySimpleJWT\Token;

date_default_timezone_set("Asia/Jakarta");

function getKecamatan($lat,$lng){
    $latlng=$lat.",".$lng;
    $baseURL="https://maps.googleapis.com/maps/api/geocode/json?latlng=";
    $apiURL=$baseURL.$latlng."&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw&result_type=administrative_area_level_3";
    $ch = curl_init(); 
    curl_setopt($ch, CURLOPT_URL, $apiURL); 
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch,CURLOPT_HTTPHEADER,array (
        "Accept: application/json"
    ));
    $curl_response = curl_exec($ch);  
    $json = json_decode(utf8_encode($curl_response), true);
    curl_close($ch);
    $kecamatan=$json["results"][0]["address_components"][0]["short_name"];
    return $kecamatan;
}

function sendOneSignalNotification($number,$content,$heading,$data){
    $curl = curl_init();
    $fields = array(
        'app_id' => "bba49751-5018-421d-82f5-fe83cc866ce6",
        'filters' => array(array("field" => "tag", "key" => "no_handphone", "relation" => "=", "value" => $number)),
        'contents' => $content,
        'headings' => $heading,
        'data'=>$data
    );
    $fields = json_encode($fields);
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, "https://onesignal.com/api/v1/notifications");
    curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json; charset=utf-8',
                                            'Authorization: Basic YjAwNDM5MTQtNjJmZS00ZjMxLWFiMTUtYjRjYTU4MjhlMWQ5'));
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
    curl_setopt($ch, CURLOPT_HEADER, FALSE);
    curl_setopt($ch, CURLOPT_POST, TRUE);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $fields);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
    $res = curl_exec($ch);
    curl_close($ch);
}

return function (App $app) {
    $container = $app->getContainer();
    $container['upload_directory'] = __DIR__ . '/uploads';
    $dotenv = Dotenv\Dotenv::createImmutable(__DIR__);
    $dotenv->load();
    

    $app->group('/settings', function() use($app){

        $app->get('/getAllKantorPolisi', function ($request, $response) {   
            $sql = "SELECT * FROM kantor_polisi";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });

        $app->get('/getKecamatan',function ($request,$response){
            $sql="SELECT * FROM kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getKategoriLostFound', function ($request, $response) {
            $sql="SELECT * FROM setting_kategori_lostfound";
            $stmt=$this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getKategoriKriminalitas', function ($request, $response) {
            $sql="SELECT * FROM setting_kategori_kriminalitas";
            $stmt=$this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/checkKecamatanAvailable', function ($request, $response) {
            $lat = $request->getQueryParam('lat');
            $lng = $request->getQueryParam('lng');
            $kecamatan=getKecamatan($lat,$lng);
            $sql="SELECT id_kecamatan FROM kecamatan where nama_kecamatan LIKE '%$kecamatan%'";
            $stmt=$this->db->prepare($sql);
            $stmt->execute();
            $id_kecamatan = $stmt->fetchColumn();
            if($id_kecamatan==null){
                return $response->withJson(["status"=>"400","message"=>"Aplikasi ini hanya menjangkau area Surabaya saja"]);
            }else{
                return $response->withJson(["status"=>"1","id_kecamatan"=>$id_kecamatan]);
            }
        });
    });
    
    $app->post('/checkLogin', function ($request, $response) {
        $body = $request->getParsedBody();
        $sql = "SELECT u.id_user,u.password_user,u.nama_user,u.telpon_user,u.status_user,u.premium_available_until,u.lokasi_aktif_user,u.id_kecamatan_user,k.nama_kecamatan AS kecamatan_user ,u.status_aktif_user
                FROM user u LEFT JOIN kecamatan k on k.id_kecamatan=u.id_kecamatan_user WHERE u.telpon_user=:telpon_user";
        $stmt = $this->db->prepare($sql);
        $stmt->execute([":telpon_user"=>$body["telpon_user"]]);
        $result = $stmt->fetch();
        if($result!=null){
            if(password_verify($body["password_user"],$result["password_user"])){
                $payload = array(
                    "iss" => "slim-framework",
                    "sub" => $result["id_user"],
                    "iat" => time(),
                    "exp" => time()+86400*7
                );
                $token = Token::customPayload($payload, $_ENV['JWT_SECRET']);
                return $response->withJson(["status" => "200", "data" => $result,"token"=>$token]);
            }else{
                return $response->withJson(["status" => "400", "message" =>"Password yang dimasukkan salah"]);
            }
        }else{  
            return $response->withJson(["status" => "404", "message" => "Nomor belum terdaftar"]);  
        }
    });

    $app->post('/registerUser', function ($request, $response) {
        $body = $request->getParsedBody();
        $sql="SELECT COUNT(*) from user where telpon_user='".$body["telpon_user"]."'";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $telpon_kembar = $stmt->fetchColumn();
        if($telpon_kembar==1){
            return $response->withJson(["status"=>"400","message"=>"No. Handphone yang dimasukkan telah terpakai"]);
        }else{
            $alamat_available=$body["alamat_available"];
            $lat_user=null;
            $lng_user=null;
            $lokasi_aktif_user=null; 
            $geohash_lokasi_aktif_user=null;        
            if($alamat_available=="1"){
                $lat_user=$body["lat_user"];
                $lng_user=$body["lng_user"];
                $lokasi_aktif_user=$body["lokasi_aktif_user"];
            }
            $sql = "INSERT INTO user (telpon_user, password_user, nama_user, status_user, active_lat_user,active_lng_user,lokasi_aktif_user,created_at,status_aktif_user) VALUE (:telpon_user, :password_user, :nama_user, :status_user, :lat,:lng,:lokasi,:created_at,:status)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":password_user"=>password_hash($body["password_user"], PASSWORD_BCRYPT),
                ":nama_user" => $body["nama_user"],
                ":telpon_user" => $body["telpon_user"],
                ":status_user"=>0,
                ":lat"=>$lat_user,
                ":lng"=>$lng_user,
                ":lokasi"=>$lokasi_aktif_user,
                ":created_at"=>date('Y').date('m').date('d'),
                ":status"=>1
            ];
            if($stmt->execute($data)){
                $sql="SELECT * FROM user WHERE id_user=:id_user";
                $stmt = $this->db->prepare($sql);
                $stmt->execute([":id_user" => $this->db->lastInsertId()]);
                $user=$stmt->fetch();
                $payload = array(
                    "iss" => "slim-framework",
                    "sub" => $user["id_user"],
                    "iat" => time(),
                    "exp" => time()+86400*7
                );
                $token = Token::customPayload($payload, $_ENV['JWT_SECRET']);
                return $response->withJson(["status" => "1","message"=>"Register akun berhasil!","data"=>$user,"token"=>$token]);
            }else{
                return $response->withJson(["status" => "99","message"=>"Register gagal, silahkan coba beberapa saat lagi"]);
            }
        }
    });

    $app->post('/sessionSignIn', function ($request, $response) {
        $body=$request->getParsedBody();
        $result = Token::validate($body["token"], $_ENV['JWT_SECRET']);
        if($result==true){
            $payload=Token::getPayload($body["token"], $_ENV['JWT_SECRET']);
            $sql ="SELECT u.id_user,u.nama_user,u.telpon_user,u.status_user,u.premium_available_until,u.lokasi_aktif_user,u.id_kecamatan_user,k.nama_kecamatan AS kecamatan_user ,u.status_aktif_user
                    FROM user u LEFT JOIN kecamatan k on k.id_kecamatan=u.id_kecamatan_user WHERE u.id_user=:id_user";
            $stmt = $this->db->prepare($sql);   
            $stmt->execute([":id_user" => $payload["sub"]]);
            $user=$stmt->fetch();
            return $response->withJson(["status"=>"1","message"=>"Token verified","data"=>$user]);
        }else{
            return $response->withJson(["status"=>"400","message"=>"Token not verified"]);
        }
    });
   
    $app->post('/loginAdmin',function($request,$response){
        $body=$request->getParsedBody();
        $email=$body["username"];
        $password=$body["password"];
        if($email==$_ENV["ADMIN_USERNAME"] && $password=$_ENV["ADMIN_PASSWORD"]){
            $payload = array(
                "iss" => "slim-framework",
                "sub" =>"admin",
                "iat" => time(),
                "exp" => time()+7200
            );
            $token = Token::customPayload($payload, $_ENV['JWT_SECRET']);
            return $response->withJson(["status" => "200","token"=>$token]);
        }else{
            return $response->withJson(["status"=>"400","message"=>"Username atau password salah"]);
        }  
    });
    
    $app->group('/kepalaKeamanan',function() use($app){
        $app->get('/getLaporanLostFound/{id_kecamatan}',function($request,$response,$args){
            $id_kecamatan=$args["id_kecamatan"];
            $sql="SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,skl.nama_kategori AS jenis_barang,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                JOIN user u ON lf.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                JOIN setting_kategori_lostfound skl on skl.id_kategori=lf.id_kategori_barang
                WHERE lf.id_kecamatan=:id_kecamatan
                GROUP BY lf.id_laporan 
                ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_kecamatan"=>$id_kecamatan]);
            $result=$stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanKriminalitasKecamatan/{id_kecamatan}',function($request,$response,$args){
            $id_kecamatan=$args["id_kecamatan"];
            $sql="SELECT COUNT(*) FROM laporan_kriminalitas WHERE id_kecamatan=:id_kecamatan AND status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_kecamatan"=>$id_kecamatan]);
            $result=$stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getLaporanKriminalitas/{id_kecamatan}/{page}',function($request,$response,$args){
            $id_kecamatan=$args["id_kecamatan"];
            $page=$args["page"];
            $offset= intval($page)*5;
            $sql="SELECT lk.id_laporan,lk.judul_laporan,lk.status_laporan,skk.nama_kategori AS jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor, COUNT(kl.id_laporan) AS jumlah_komentar,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lk.id_laporan=kl.id_laporan
                JOIN setting_kategori_kriminalitas skk on skk.id_kategori=lk.id_kategori_kejadian
                WHERE lk.id_kecamatan=:id_kecamatan AND lk.status_laporan=1
                GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC LIMIT 5 OFFSET $offset";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_kecamatan"=>$id_kecamatan]);
            $result=$stmt->fetchAll();
            return $response->withJson($result);
        });
    })->add(function ($request, $response, $next) {
        $headers = $request->getHeader("Authorization");
        if($headers!=null){
            $result = Token::validate($headers[0], $_ENV['JWT_SECRET']);
            if($result==false){
                return $response->withJson(["status"=>"400","message"=>"Token not valid"]);
            }else{
                $response = $next($request, $response);
                return $response;
            }         
        }else{
            return $response->withJson(["status"=>"404","message"=>"Token not found"]);
        }
    });

    $app->group('/laporan', function() use($app){
        $app->post('/insertLaporanLostFound', function(Request $request, Response $response) {
            $body = $request->getParsedBody();
            $waktu=$body["waktu_laporan"];
            $tanggal=$body["tanggal_laporan"];
            $date=strtotime($tanggal);
            $id_laporan="LF".date('d',$date).date('m',$date).date('Y',$date);
            $sql="SELECT COUNT(*)+1 from laporan_lostfound_barang where id_laporan like'%$id_laporan%'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();          
            $uploadedFiles = $request->getUploadedFiles();
            $id_laporan=$id_laporan.str_pad($result,5,"0",STR_PAD_LEFT);    
            $filename="no-image.png";
            if($uploadedFiles!=null){
                $uploadedFile = $uploadedFiles['image'];
                $extension = pathinfo($uploadedFile->getClientFilename(), PATHINFO_EXTENSION);
                $filename=$id_laporan.".".$extension;
            }       
            $sql = "INSERT INTO laporan_lostfound_barang VALUES(:id_laporan,:judul_laporan,:jenis_laporan,:id_kategori_barang,:tanggal_laporan,:waktu_laporan,:alamat_laporan,:lat_laporan,:lng_laporan,:deskripsi_barang,:id_user_pelapor,:status_laporan,:id_kecamatan,:thumbnail_gambar) ";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $id_laporan,
                ":judul_laporan"=>$body["judul_laporan"],
                ":jenis_laporan" => $body["jenis_laporan"],
                ":id_kategori_barang"=> $body["id_kategori_barang"],
                ":tanggal_laporan"=>$tanggal,
                ":waktu_laporan"=>$waktu,
                ":alamat_laporan"=>$body["alamat_laporan"],
                ":lat_laporan"=>$body["lat_laporan"],
                ":lng_laporan"=>$body["lng_laporan"],
                ":deskripsi_barang"=>$body["deskripsi_barang"],
                ":id_user_pelapor"=>$body["id_user_pelapor"],
                ":status_laporan"=>0,
                ":id_kecamatan"=>$body["id_kecamatan"],
                ":thumbnail_gambar"=>$filename
            ];
            if($stmt->execute($data)){
                if($uploadedFiles!=null){
                    $directory = $this->get('settings')['upload_directory'];
                    $uploadedFile->moveTo($directory . DIRECTORY_SEPARATOR . $filename);
                }
                return $response->withJson(["status"=>"1","message"=>"Tambah laporan berhasil"]);
            }else{
                return $response->withJson(["status"=>"99","message"=>"Tambah laporan gagal, silahkan coba beberapa saat lagi"]);
            }
        });

        $app->post('/insertLaporanKriminalitas', function(Request $request, Response $response,$args) {
            $body = $request->getParsedBody();
            $waktu=$body["waktu_laporan"];
            $tanggal=$body["tanggal_laporan"];
            $date=strtotime($tanggal);
            $id_laporan="CR".date('d',$date).date('m',$date).date('Y',$date);
            $sql="SELECT COUNT(*)+1 from laporan_kriminalitas where id_laporan like'%$id_laporan%'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();     
            $uploadedFiles = $request->getUploadedFiles();
            $id_laporan=$id_laporan.str_pad($result,5,"0",STR_PAD_LEFT);    
            $filename="no-image.png";
            if($uploadedFiles!=null){
                $uploadedFile = $uploadedFiles['image'];
                $extension = pathinfo($uploadedFile->getClientFilename(), PATHINFO_EXTENSION);
                $filename=$id_laporan.".".$extension;
            }      
            $sql = "INSERT INTO laporan_kriminalitas VALUES(:id_laporan,:judul_laporan,:id_kategori_kejadian,:deskripsi_kejadian,:tanggal_laporan,:waktu_laporan,:alamat_laporan,:lat_laporan,:lng_laporan,:id_user_pelapor,:status_laporan,:id_kecamatan,:thumbnail_gambar) ";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $id_laporan,
                ":judul_laporan"=>$body["judul_laporan"],
                ":id_kategori_kejadian" => $body["id_kategori_kejadian"],
                ":deskripsi_kejadian"=>$body["deskripsi_kejadian"],
                ":tanggal_laporan"=>$tanggal,
                ":waktu_laporan"=>$waktu,
                ":alamat_laporan"=>$body["alamat_laporan"],
                ":lat_laporan"=>$body["lat_laporan"],
                ":lng_laporan"=>$body["lng_laporan"],
                ":id_user_pelapor"=>$body["id_user_pelapor"],
                ":status_laporan"=>0,
                ":id_kecamatan"=>$body["id_kecamatan"],
                ":thumbnail_gambar"=>$filename
            ];
            if($stmt->execute($data)){
                if($uploadedFiles!=null){
                    $directory = $this->get('settings')['upload_directory'];
                    $uploadedFile->moveTo($directory . DIRECTORY_SEPARATOR . $filename);
                }
                return $response->withJson(["status"=>"1","message"=>"Tambah laporan berhasil"]);
            }else{
                return $response->withJson(["status"=>"99","message"=>"Tambah laporan gagal, silahkan coba beberapa saat lagi"]);
            }
        });

        $app->put('/cancelLaporan/{kategori_laporan}/{id_laporan}',function($request,$response,$args){
            $kategori_laporan=$args["kategori_laporan"];
            $id_laporan=$args["id_laporan"];
            $jenis_laporan=$kategori_laporan=="0"? "laporan_lostfound_barang" : "laporan_kriminalitas";
            $sql="UPDATE ".$jenis_laporan." SET status_laporan=2 WHERE id_laporan=:id_laporan";
            $stmt=$this->db->prepare($sql);
            if($stmt->execute([":id_laporan"=>$id_laporan])){
                return $response->withJson(["status"=>"1","message"=>"Berhasil membatalkan laporan"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal membatalkan laporan"]);
            }
        });

        $app->get('/getKomentarLaporan/{id_laporan}', function ($request, $response,$args) {
            $id_laporan=$args["id_laporan"];
            $sql = "SELECT kl.id_komentar,kl.id_laporan,kl.isi_komentar,kl.waktu_komentar,u.nama_user AS nama_user_komentar
                    FROM komentar_laporan kl, user u 
                    WHERE kl.id_user_komentar=u.id_user and kl.id_laporan=:id_laporan
                    ORDER BY kl.waktu_komentar DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getHeadlineLaporanKriminalitas', function ($request, $response) {
            $sql = "SELECT 0 AS kategori_laporan,lk.id_laporan, lk.judul_laporan, skk.nama_kategori AS jenis_kejadian, lk.deskripsi_kejadian, lk.tanggal_laporan, lk.waktu_laporan, lk.alamat_laporan, lk.lat_laporan, lk.lng_laporan, lk.id_user_pelapor, u.nama_user AS nama_user_pelapor, lk.status_laporan, Count(DISTINCT kl.id_komentar)  AS jumlah_komentar, Count(DISTINCT klk.id_konfirmasi) AS jumlah_konfirmasi, lk.thumbnail_gambar AS thumbnail_gambar 
            FROM laporan_kriminalitas lk JOIN user u ON lk.id_user_pelapor = u.id_user 
            LEFT JOIN konfirmasi_laporan_kriminalitas klk ON klk.id_laporan = lk.id_laporan 
            LEFT JOIN komentar_laporan kl ON kl.id_laporan = lk.id_laporan 
            JOIN setting_kategori_kriminalitas skk ON skk.id_kategori = lk.id_kategori_kejadian 
            WHERE  lk.status_laporan = 1 
            GROUP  BY lk.id_laporan 
            ORDER  BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC 
            LIMIT  5";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->post('/konfirmasiLaporanKriminalitas',function ($request,$response){
            $body = $request->getParsedBody();
            $sql="INSERT INTO konfirmasi_laporan_kriminalitas(id_laporan,id_user) VALUES(:id_laporan,:id_user)";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":id_laporan"=>$body["id_laporan"],
                ":id_user"=>$body["id_user"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1", "message" => "Konfirmasi Laporan Berhasil"]);
            }else{
                return $response->withJson(["status" => "400", "message" => "Konfirmasi Laporan Gagal"]);
            }
        });

        $app->get('/checkKonfirmasiLaporan',function ($request,$response){
            $id_laporan = $request->getQueryParam('id_laporan');
            $id_user = $request->getQueryParam('id_user');
            $sql="SELECT COUNT(*) FROM konfirmasi_laporan_kriminalitas WHERE id_laporan=:id_laporan AND id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":id_laporan"=>$id_laporan,
                ":id_user"=>$id_user
            ];
            $stmt->execute($data);
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });
    
        $app->get('/getHeadlineLaporanLostFound', function ($request, $response) {
            $sql = "SELECT 1 as kategori_laporan,lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,skl.nama_kategori AS jenis_barang,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,lf.status_laporan, COUNT(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                    JOIN user u ON lf.id_user_pelapor=u.id_user 
                    LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                    JOIN setting_kategori_lostfound skl on skl.id_kategori=lf.id_kategori_barang
                    WHERE lf.status_laporan=1
                    GROUP BY lf.id_laporan 
                    ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC LIMIT 5";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getLaporanLostFound/{page}', function ($request, $response,$args) {
            $page=$args["page"];
            $offset= intval($page)*5;
            $sql = "SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.status_laporan,skl.nama_kategori AS jenis_barang,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                    JOIN user u ON lf.id_user_pelapor=u.id_user 
                    LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                    JOIN setting_kategori_lostfound skl on skl.id_kategori=lf.id_kategori_barang
                    WHERE lf.status_laporan=1
                    GROUP BY lf.id_laporan
                    ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC LIMIT 5 OFFSET $offset ";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });
    
        $app->get('/getLaporanLostFoundWithFilter/{page}', function ($request, $response,$args) {
            $tanggal_awal=$request->getQueryParam('tanggal_awal');
            $tanggal_akhir=$request->getQueryParam('tanggal_akhir');
            $array_barang=$request->getQueryParam('id_barang');
            $id_kecamatan=$request->getQueryParam('id_kecamatan');
            $filter_kecamatan=$id_kecamatan=="0" ? " IS NOT NULL" : "=$id_kecamatan";
            $jenis_laporan=$request->getQueryParam('jenis_laporan');
            $page=$args["page"];
            $offset= intval($page)*5;
            $sql = "SELECT lf.id_laporan,lf.judul_laporan,skl.nama_kategori AS jenis_barang,lf.jenis_laporan,lf.status_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
            JOIN user u ON lf.id_user_pelapor=u.id_user 
            LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
            JOIN setting_kategori_lostfound skl ON skl.id_kategori=lf.id_kategori_barang
            WHERE lf.status_laporan=1 AND lf.jenis_laporan IN ($jenis_laporan) AND lf.id_kategori_barang IN ($array_barang) AND lf.id_kecamatan ".$filter_kecamatan." AND (lf.tanggal_laporan BETWEEN :tanggal_awal AND :tanggal_akhir)
            GROUP BY lf.id_laporan 
            ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC LIMIT 5 OFFSET $offset";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":tanggal_awal"=>$tanggal_awal,
                ":tanggal_akhir"=>$tanggal_akhir
            ];
            $stmt->execute($data);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanKriminalitas',function ($request,$response,$args){
            $sql="SELECT COUNT(*) from laporan_kriminalitas WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getJumlahLaporanLostFound',function ($request,$response,$args){
            $sql="SELECT COUNT(*) from laporan_lostfound_barang WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getJumlahLaporanLostFoundWithFilter',function ($request,$response){
            $tanggal_awal=$request->getQueryParam('tanggal_awal');
            $tanggal_akhir=$request->getQueryParam('tanggal_akhir');
            $array_barang=$request->getQueryParam('id_barang');
            $id_kecamatan=$request->getQueryParam('id_kecamatan');
            $jenis_laporan=$request->getQueryParam('jenis_laporan');
            $filter_kecamatan=$id_kecamatan=="0" ? " IS NOT NULL" : "=$id_kecamatan";
            $sql="SELECT COUNT(*) from laporan_lostfound_barang WHERE status_laporan=1 AND jenis_laporan IN ($jenis_laporan) AND id_kategori_barang IN ($array_barang) AND id_kecamatan ".$filter_kecamatan." AND (tanggal_laporan BETWEEN :tanggal_awal AND :tanggal_akhir) ";
            $data=[
                ":tanggal_awal"=>$tanggal_awal,
                ":tanggal_akhir"=>$tanggal_akhir
            ];
            $stmt = $this->db->prepare($sql);
            $stmt->execute($data);
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getJumlahLaporanKriminalitasWithFilter',function ($request,$response){
            $tanggal_awal=$request->getQueryParam('tanggal_awal');
            $tanggal_akhir=$request->getQueryParam('tanggal_akhir');
            $array_kejadian=$request->getQueryParam('id_kejadian');
            $id_kecamatan=$request->getQueryParam('id_kecamatan');
            $filter_kecamatan=$id_kecamatan=="0" ? " IS NOT NULL" : "=$id_kecamatan";
            $sql="SELECT COUNT(*) from laporan_kriminalitas WHERE status_laporan=1 AND id_kategori_kejadian IN ($array_kejadian) AND id_kecamatan" .$filter_kecamatan." AND (tanggal_laporan BETWEEN :tanggal_awal AND :tanggal_akhir) ";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":tanggal_awal"=>$tanggal_awal,
                ":tanggal_akhir"=>$tanggal_akhir
            ];
            $stmt->execute($data);
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getLaporanKriminalitas/{page}', function ($request, $response,$args) {
            $page=$args["page"];
            $offset= intval($page)*5;
            $sql = "SELECT lk.id_laporan,lk.judul_laporan,skk.nama_kategori AS jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor,lk.status_laporan,COUNT(klk.id_laporan) AS jumlah_konfirmasi,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                    JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                    LEFT JOIN konfirmasi_laporan_kriminalitas klk ON lk.id_laporan=klk.id_laporan 
                    JOIN setting_kategori_kriminalitas skk on skk.id_kategori=lk.id_kategori_kejadian 
                    WHERE lk.status_laporan=1 GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC LIMIT 5 OFFSET $offset";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getLaporanKriminalitasWithFilter/{page}', function ($request, $response,$args) {
            $tanggal_awal=$request->getQueryParam('tanggal_awal');
            $tanggal_akhir=$request->getQueryParam('tanggal_akhir');
            $array_kejadian=$request->getQueryParam('id_kejadian');
            $id_kecamatan=$request->getQueryParam('id_kecamatan');
            $page=$args["page"];
            $offset= intval($page)*5;
            $filter_kecamatan=$id_kecamatan=="0" ? " IS NOT NULL" : "=$id_kecamatan";
            $sql = "SELECT lk.id_laporan,lk.judul_laporan,skk.nama_kategori AS jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor,lk.status_laporan,COUNT(klk.id_laporan) AS jumlah_konfirmasi,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                    JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                    LEFT JOIN konfirmasi_laporan_kriminalitas klk ON lk.id_laporan=klk.id_laporan 
                    JOIN setting_kategori_kriminalitas skk on skk.id_kategori=lk.id_kategori_kejadian 
                    WHERE lk.status_laporan=1 AND lk.id_kategori_kejadian IN ($array_kejadian) AND lk.id_kecamatan" .$filter_kecamatan." AND (lk.tanggal_laporan BETWEEN :tanggal_awal AND :tanggal_akhir)
                    GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC LIMIT 5 OFFSET $offset";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":tanggal_awal"=>$tanggal_awal,
                ":tanggal_akhir"=>$tanggal_akhir
            ];
            $stmt->execute($data);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

    })->add(function ($request, $response, $next) {
        $headers = $request->getHeader("Authorization");
        if($headers!=null){
            $result = Token::validate($headers[0], $_ENV['JWT_SECRET']);
            if($result==false){
                return $response->withJson(["status"=>"400","message"=>"Token not valid"]);
            }else{
                $response = $next($request, $response);
                return $response;
            }         
        }else{
            return $response->withJson(["status"=>"404","message"=>"Token not found"]);
        }
    });

    $app->group('/admin', function() use($app){
        $app->get('/getReportTransaksi', function ($request, $response) {
            $sql = "SELECT os.id_order,os.order_ammount,os.order_date,u.nama_user AS nama_user,u.telpon_user AS telpon_user FROM order_subscription os, user u WHERE u.id_user=os.id_user ORDER BY os.order_date DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahUser', function ($request, $response) {
            $sql = "SELECT COUNT(*) FROM user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getJumlahLaporanLostFoundPerJenis', function ($request, $response) {
            $sql = "SELECT CASE WHEN lf.jenis_laporan=0 THEN 'Penemuan Barang' ELSE 'Kehilangan Barang' END as kategori_laporan ,COUNT(lf.jenis_laporan) as jumlah_laporan from laporan_lostfound_barang lf  group by lf.jenis_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getTotalLaporanLostFound', function ($request, $response) {
            $sql = "SELECT COUNT(*) FROM laporan_lostfound_barang";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getTotalLaporanKriminalitas', function ($request, $response) {
            $sql = "SELECT COUNT(*) FROM laporan_kriminalitas";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getDataLaporanLostFoundForChartKecamatan', function ($request, $response) {
            $sql = "SELECT k.nama_kecamatan,COUNT(lf.id_kecamatan) AS jumlah_laporan FROM kecamatan k LEFT JOIN laporan_lostfound_barang lf ON k.id_kecamatan=lf.id_kecamatan GROUP BY k.id_kecamatan ORDER BY jumlah_laporan DESC LIMIT 5";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getDataLaporanKriminalitasForChartKecamatan', function ($request, $response) {
            $sql = "SELECT k.nama_kecamatan,COUNT(lk.id_kecamatan) AS jumlah_laporan FROM kecamatan k LEFT JOIN laporan_kriminalitas lk ON k.id_kecamatan=lk.id_kecamatan GROUP BY k.id_kecamatan ORDER BY jumlah_laporan DESC LIMIT 5";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanLostFoundPerItem', function ($request, $response) {
            $sql = "SELECT skl.id_kategori, skl.nama_kategori, count(lf.id_kategori_barang) as jumlah_laporan from setting_kategori_lostfound skl LEFT JOIN laporan_lostfound_barang lf ON skl.id_kategori=lf.id_kategori_barang GROUP BY skl.id_kategori";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanKriminalitasPerKejadian', function ($request, $response) {
            $sql = "SELECT skk.id_kategori, skk.nama_kategori, count(lk.id_kategori_kejadian) as jumlah_laporan from setting_kategori_kriminalitas skk LEFT JOIN laporan_kriminalitas lk ON skk.id_kategori=lk.id_kategori_kejadian GROUP BY skk.id_kategori";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanKriminalitas', function ($request, $response) {
            $sql = "SELECT COUNT(*) from laporan_kriminalitas WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getJumlahLaporanLostFound', function ($request, $response) {
            $sql = "SELECT COUNT(*) from laporan_lostfound_barang WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["count"=>$result]);
        });

        $app->get('/getMarkerLocationLaporanKriminalitas', function ($request, $response) {
            $sql = "SELECT lat_laporan AS lat, lng_laporan AS lng from laporan_kriminalitas WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getMarkerLocationLaporanLostFound', function ($request, $response) {
            $sql = "SELECT lat_laporan AS lat, lng_laporan AS lng from laporan_lostfound_barang WHERE status_laporan=1";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getJumlahLaporanKriminalitasKecamatan', function ($request, $response) {
            $sql = "SELECT k.nama_kecamatan,COUNT(lk.id_kecamatan) AS jumlah_laporan FROM kecamatan k LEFT JOIN laporan_kriminalitas lk ON k.id_kecamatan=lk.id_kecamatan GROUP BY k.id_kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            $max=0;
            foreach($result as $k){
                if($k["jumlah_laporan"]>$max){
                    $max=$k["jumlah_laporan"];
                }
            }
            return $response->withJson(["data"=>$result,"max"=>$max]);
        });

        $app->get('/getJumlahLaporanLostFoundKecamatan', function ($request, $response) {
            $sql = "SELECT k.nama_kecamatan,COUNT(lf.id_kecamatan) AS jumlah_laporan FROM kecamatan k LEFT JOIN laporan_lostfound_barang lf ON k.id_kecamatan=lf.id_kecamatan GROUP BY k.id_kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            $max=0;
            foreach($result as $k){
                if($k["jumlah_laporan"]>$max){
                    $max=$k["jumlah_laporan"];
                }
            }
            return $response->withJson(["data"=>$result,"max"=>$max]);
        });
        
        $app->put('/verifikasiLaporanLostFound/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args["id_laporan"];
            $sql="UPDATE laporan_lostfound_barang SET status_laporan=1 WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute(["id_laporan"=>$id_laporan]);
            if($stmt->execute()){
                $sql="SELECT lf.id_laporan,lf.judul_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.deskripsi_barang,lf.thumbnail_gambar,lf.lat_laporan,lf.lng_laporan,lf.jenis_laporan,lf.alamat_laporan,lf.id_user_pelapor,lf.status_laporan,u.nama_user 
                    AS nama_user_pelapor,skl.nama_kategori AS jenis_barang 
                    FROM laporan_lostfound_barang lf,setting_kategori_lostfound skl,user u WHERE lf.id_laporan=:id_laporan AND lf.id_kategori_barang=skl.id_kategori AND lf.id_user_pelapor=u.id_user";
                $stmt= $this->db->prepare($sql);
                $stmt->execute(["id_laporan"=>$id_laporan]);
                $laporan=$stmt->fetch();
                $sql="SELECT telpon_user FROM user WHERE calcDistance(active_lat_user,active_lng_user,:lat_laporan,:lng_laporan)<=3000";
                $stmt= $this->db->prepare($sql);
                $data=[
                    ":lat_laporan"=>$laporan["lat_laporan"],
                    ":lng_laporan"=>$laporan["lng_laporan"]
                ];
                $stmt->execute($data);
                $result=$stmt->fetchAll();
                $display_jenis_laporan= $laporan["jenis_laporan"]==0 ? "penemuan" : "kehilangan";
                $message="Ada user yang telah melaporkan ".$display_jenis_laporan." ".$laporan["jenis_barang"]." di ".$laporan["alamat_laporan"];
                $message=$laporan["jenis_laporan"] == 0 ? $message : $message.". Segera hubungi user yang bersangkutan apabila anda mempunyai informasi mengenai laporan tersebut.";
                $content = array(
                    "en" => $message
                );
                $heading = array(
                    "en" => "Cek laporan " .$display_jenis_laporan." barang baru didaerahmu!"
                );
                $tag=$laporan["jenis_laporan"] == 0 ? "Penemuan barang":"Kehilangan barang";
                $jenis_laporan=$laporan["jenis_laporan"] == 0 ? "Penemuan ".$laporan["jenis_barang"] : "Kehilangan ".$laporan["jenis_barang"]; 
                $data=array(
                    "page"=>"1",
                    "id_laporan"=>$laporan["id_laporan"],
                    "judul_laporan"=>$laporan["judul_laporan"],
                    "jenis_laporan"=>$jenis_laporan,
                    "alamat_laporan"=>$laporan["alamat_laporan"],
                    "tanggal_laporan"=>$laporan["tanggal_laporan"],
                    "waktu_laporan"=>$laporan["waktu_laporan"],
                    "lat_laporan"=>$laporan["lat_laporan"],
                    "lng_laporan"=>$laporan["lng_laporan"],
                    "deskripsi_laporan"=>$laporan["deskripsi_barang"],
                    "tag"=>$tag,
                    "id_user_pelapor"=>$laporan["id_user_pelapor"],
                    "nama_user_pelapor"=>$laporan["nama_user_pelapor"],
                    "status_laporan"=>$laporan["status_laporan"],
                    "thumbnail_gambar"=>$laporan["thumbnail_gambar"],
                );
                foreach($result as $user){
                    sendOneSignalNotification($user["telpon_user"],$content,$heading,$data);
                }
                return $response->withJson(["status"=>"1","message"=>"Laporan berhasil dikonfirmasi",]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Laporan gagal dikonfirmasi"]);  
            }
        });

        $app->put('/declineLaporanKriminalitas/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args["id_laporan"];
            $sql="UPDATE laporan_kriminalitas SET status_laporan=99 WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute(["id_laporan"=>$id_laporan])){
                return $response->withJson(["status"=>"1","message"=>"Laporan berhasil ditolak"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Laporan gagal ditolak"]);
            }
        });

        $app->put('/declineLaporanLostFound/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args["id_laporan"];
            $sql="UPDATE laporan_lostfound_barang SET status_laporan=99 WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute(["id_laporan"=>$id_laporan])){
                return $response->withJson(["status"=>"1","message"=>"Laporan berhasil ditolak"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Laporan gagal ditolak"]);
            }
        });

        $app->put('/verifikasiLaporanKriminalitas/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args["id_laporan"];
            $sql="UPDATE laporan_kriminalitas SET status_laporan=1 WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute(["id_laporan"=>$id_laporan]);
            if($stmt->execute()){
                $sql="SELECT lk.id_laporan,lk.judul_laporan,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.deskripsi_kejadian,lk.thumbnail_gambar,lk.lat_laporan,lk.lng_laporan,lk.alamat_laporan,lk.id_user_pelapor,lk.status_laporan,u.nama_user AS nama_user_pelapor,
                     skk.nama_kategori AS jenis_kejadian,lk.id_kecamatan,COUNT(klk.id_laporan) AS jumlah_konfirmasi 
                     FROM laporan_kriminalitas lk LEFT JOIN konfirmasi_laporan_kriminalitas klk ON klk.id_laporan=lk.id_laporan 
                     JOIN setting_kategori_kriminalitas skk ON lk.id_kategori_kejadian=skk.id_kategori JOIN user u ON lk.id_user_pelapor=u.id_user WHERE lk.id_laporan=:id_laporan";
                $stmt= $this->db->prepare($sql);
                $stmt->execute(["id_laporan"=>$id_laporan]);
                $laporan=$stmt->fetch();
                $sql="SELECT telpon_user FROM user WHERE id_kecamatan_user=:id_kecamatan_user AND status_user=2";
                $stmt= $this->db->prepare($sql);
                $stmt->execute([":id_kecamatan_user"=>$laporan["id_kecamatan"]]);
                $daftar_kepala_keamanan=$stmt->fetchAll();
                $message="Ada masyarakat yang telah membuat laporan tentang ".$laporan["jenis_kejadian"];
                $message_location="Lokasi kejadian di ".$laporan["alamat_laporan"];
                $content = array(
                    "en" => $message." di area pengawasanmu. ".$message_location
                );
                $heading = array(
                    "en" => "Cek laporan baru di area pengawasan anda!"
                );
                $data_onesignal=array(
                    "page"=>"1",
                    "id_laporan"=>$laporan["id_laporan"],
                    "judul_laporan"=>$laporan["judul_laporan"],
                    "jenis_laporan"=>$laporan["jenis_kejadian"],
                    "alamat_laporan"=>$laporan["alamat_laporan"],
                    "tanggal_laporan"=>$laporan["tanggal_laporan"],
                    "waktu_laporan"=>$laporan["waktu_laporan"],
                    "lat_laporan"=>$laporan["lat_laporan"],
                    "lng_laporan"=>$laporan["lng_laporan"],
                    "deskripsi_laporan"=>$laporan["deskripsi_kejadian"],
                    "tag"=>"kriminalitas",
                    "id_user_pelapor"=>$laporan["id_user_pelapor"],
                    "nama_user_pelapor"=>$laporan["nama_user_pelapor"],
                    "status_laporan"=>$laporan["status_laporan"],
                    "thumbnail_gambar"=>$laporan["thumbnail_gambar"],
                    "jumlah_konfirmasi"=>$laporan["jumlah_konfirmasi"]
                );
                foreach($daftar_kepala_keamanan as $kepala_keamanan){
                    sendOneSignalNotification($kepala_keamanan["telpon_user"],$content,$heading,$data_onesignal);
                }
                $sql="SELECT telpon_user FROM user WHERE calcDistance(last_lat_user,last_lng_user,:lat_laporan,:lng_laporan)<=3000";
                $stmt= $this->db->prepare($sql);
                $data=[
                    ":lat_laporan"=>$laporan["lat_laporan"],
                    ":lng_laporan"=>$laporan["lng_laporan"]
                ];
                $stmt->execute($data);
                $daftar_user=$stmt->fetchAll();
                $content = array(
                    "en" => $message." dalam radius 3km dari lokasimu sekarang. ".$message_location.". Berhati-hatilah apabila melewati area tersebut!"
                );
                $heading = array(
                    "en" => "Ada laporan kriminalitas baru di dekatmu!"
                );
                foreach($daftar_user as $user){
                    sendOneSignalNotification($user["telpon_user"],$content,$heading,$data_onesignal);
                }
                return $response->withJson(["status"=>"1","message"=>"Laporan berhasil dikonfirmasi"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Laporan gagal dikonfirmasi"]);
            }
        });

        $app->get('/getAllUser', function ($request, $response) {
            $sql = "SELECT id_user,telpon_user,nama_user,status_user,status_aktif_user FROM user where status_user!=2";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });
    
        $app->get('/getLaporanLostFoundVerify',function ($request,$response){
            $sql="SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,skl.nama_kategori AS jenis_barang,k.nama_kecamatan AS kecamatan FROM laporan_lostfound_barang lf, setting_kategori_lostfound skl,kecamatan k WHERE lf.status_laporan=0 AND lf.id_kategori_barang=skl.id_kategori AND lf.id_kecamatan=k.id_kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->put('/acceptUser/{id_user}',function ($request,$response,$args){
            $id_user=$args["id_user"];
            $sql="UPDATE user SET status_aktif_user=1 WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute([":id_user"=>$id_user])){
                return $response->withJson(["status"=>"1","message"=>"Berhasil mengubah status pending user"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal mengubah status pending user"]);
            }
        });

        $app->get('/getDetailLaporanLostFound/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args['id_laporan'];
            $sql="SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.thumbnail_gambar,lf.deskripsi_barang,skl.nama_kategori AS jenis_barang,k.nama_kecamatan AS kecamatan FROM laporan_lostfound_barang lf, setting_kategori_lostfound skl,kecamatan k WHERE lf.id_laporan=:id_laporan AND lf.id_kategori_barang=skl.id_kategori AND lf.id_kecamatan=k.id_kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetch();
            return $response->withJson($result);
        });

        $app->get('/getDetailLaporanKriminalitas/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args['id_laporan'];
            $sql="SELECT lk.id_laporan,lk.judul_laporan,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.thumbnail_gambar,lk.deskripsi_kejadian,skk.nama_kategori AS jenis_kejadian,k.nama_kecamatan AS kecamatan FROM laporan_kriminalitas lk, setting_kategori_kriminalitas skk,kecamatan k WHERE lk.id_laporan=:id_laporan AND lk.id_kategori_kejadian=skk.id_kategori AND lk.id_kecamatan=k.id_kecamatan;";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetch();
            return $response->withJson($result);
        });

        $app->get('/getLaporanKriminalitasVerify',function ($request,$response){
            $sql="SELECT lk.id_laporan,lk.judul_laporan,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,skk.nama_kategori AS jenis_kejadian,k.nama_kecamatan AS kecamatan FROM laporan_kriminalitas lk, setting_kategori_kriminalitas skk,kecamatan k WHERE lk.status_laporan=0 AND lk.id_kategori_kejadian=skk.id_kategori AND lk.id_kecamatan=k.id_kecamatan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });
        
        $app->get('/getKepalaKeamanan',function ($request,$response){
            $sql="SELECT u.id_user,u.nama_user,u.telpon_user,u.status_aktif_user,k.nama_kecamatan AS kecamatan_user FROM user u, kecamatan k WHERE u.id_kecamatan_user=k.id_kecamatan AND u.status_user=2";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });
        
        $app->put('/activateUser/{id_user}',function ($request,$response,$args){
            $id_user=$args["id_user"];
            $sql="UPDATE user SET status_aktif_user=1 WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute([":id_user" => $id_user])){
                return $response->withJson(["status"=>"1","message"=>"Berhasil mengaktifkan user"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal mengaktifkan user"]);
            }
        });

        $app->put('/banUser/{id_user}',function ($request,$response,$args){
            $id_user=$args["id_user"];
            $sql="UPDATE user SET status_aktif_user=0 WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute([":id_user" => $id_user])){
                return $response->withJson(["status"=>"1","message"=>"Berhasil menonaktifkan user"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal menonaktifkan user"]);
            }
        });

        $app->put('/updateKepalaKeamanan/{id_user}',function ($request,$response,$args){
            $id_user=$args["id_user"];
            $body=$request->getParsedBody();
            $sql="UPDATE user SET nama_user=:nama_user,id_kecamatan_user=:id_kecamatan_user WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_user" =>$id_user,
                ":nama_user" => $body["nama_user"],
                ":id_kecamatan_user"=>$body["id_kecamatan_user"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status"=>"1","message"=>"Berhasil mengubah data kepala keamanan"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal mengubah data kepala keamanan"]);
            }
        });

        $app->post('/addKepalaKeamanan',function ($request,$response){
            $body=$request->getParsedBody();
            $sql="SELECT COUNT(*) FROM user WHERE telpon_user=:telpon_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":telpon_user" => $body["telpon_user"]]);
            $result=$stmt->fetchColumn();
            if($result==0){
                $sql = "INSERT INTO user (password_user, nama_user, telpon_user, status_user,id_kecamatan_user,created_at,status_aktif_user) VALUE (:password_user, :nama_user, :telpon_user, :status_user,:id_kecamatan_user,:created_at,:status_aktif_user)";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":password_user"=>password_hash($body["password_user"], PASSWORD_BCRYPT),
                    ":nama_user" => $body["nama_user"],
                    ":telpon_user" => $body["telpon_user"],
                    ":status_user"=>2,
                    ":id_kecamatan_user"=>$body["id_kecamatan_user"],
                    ":created_at"=> date("Y/m/d"),
                    ":status_aktif_user"=>1
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["status"=>"1","message"=>"Tambah kepala keamanan berhasil"]);
                }else{
                    return $response->withJson(["status"=>"99","message"=>"Tambah kepala keamanan gagal, coba beberapa saat lagi"]);
                }     
            }else{
                return $response->withJson(["status"=>"400","message"=>"Nomor handphone sudah terdaftar"]);
            }
        });
        $app->get('/getKomentarLaporan',function($request,$response){
            $sql="SELECT kl.id_komentar,kl.id_laporan,kl.isi_komentar,kl.waktu_komentar,kl.id_user_komentar,u.nama_user AS nama_user_komentar,CASE WHEN substring(kl.id_laporan,1,1)='C' THEN lk.judul_laporan ELSE lf.judul_laporan END AS judul_laporan FROM komentar_laporan kl LEFT JOIN laporan_kriminalitas lk ON lk.id_laporan=kl.id_laporan LEFT JOIN laporan_lostfound_barang lf ON lf.id_laporan=kl.id_laporan JOIN user u ON u.id_user=kl.id_user_komentar 
                ORDER BY kl.waktu_komentar DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);    
        });

        $app->delete('/deleteKomentarLaporan/{id_komentar}',function($request,$response,$args){
            $id_komentar=$args["id_komentar"];
            $sql="DELETE FROM komentar_laporan WHERE id_komentar=:id_komentar";
            $stmt = $this->db->prepare($sql);
            if($stmt->execute(["id_komentar"=>$id_komentar])){
                return $response->withJson(["status"=>"1","message"=>"Berhasil menghapus komentar"]); 
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal menghapus komentar"]);
            }
        });
    });

    $app->group('/user', function () use ($app) {
        $app->get('/getPremiumUserInformation', function ($request, $response) {
            $sql="SELECT id_user, credit_card_token,premium_available_until FROM user WHERE status_user=1";
            $stmt=$this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->put('/updateLocation/{id_user}',function ($request, $response,$args){
            $lat=$request->getQueryParam('lat');
            $lng=$request->getQueryParam('lng');
            $id_user=$args["id_user"];
            $sql="UPDATE user SET last_lat_user=:lat,last_lng_user=:lng WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":lat"=>$lat,
                ":lng"=>$lng,
                ":id_user"=>$id_user
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status"=>"1"]);
            }else{
                return $response->withJson(["status"=>"400"]);
            }
        });


        $app->put('/updateProfile/{id_user}',function ($request, $response,$args){
            $body=$request->getParsedBody();
            $password_user=$body["password_user"];
            $sql="SELECT password_user FROM user WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $args["id_user"]]);
            $result=$stmt->fetchColumn();
            if(password_verify($password_user,$result)){
                $lat_user=null;
                $lng_user=null;
                $lokasi_aktif_user=null;
                if($body["lokasi_available"]=="1"){
                    $lat_user=$body["lat_user"];
                    $lng_user=$body["lng_user"];
                    $lokasi_aktif_user=$body["lokasi_aktif_user"];
                }
                $sql="UPDATE user SET nama_user=:nama_user, active_lat_user=:lat_user, active_lng_user=:lng_user WHERE id_user=:id_user";
                $stmt=$this->db->prepare($sql); 
                $data=[
                    ":id_user"=>$args["id_user"],
                    ":nama_user"=>$body["nama_user"],
                    ":lat_user"=>$lat_user,
                    ":lng_user"=>$lng_user,
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["status"=>"1","message"=>"Berhasil memperbarui informasi"]);
                }else{
                    return $response->withJson(["status"=>"99","message"=>"Gagal memperbarui informasi, silahkan coba beberapa saat lagi"]);
                }
            }else{
                return $response->withJson(["status"=>"400","message"=>"Password tidak sesuai, gagal memperbarui informasi"]);
            }
        });

        $app->post('/sendOTP/{number}', function($request,$response,$args){
            $url = "https://numberic1.tcastsms.net:20005/sendsms?account=def_robby3&password=123456";
            $code=rand(1000,9999);
            $message="Masukkan nomor ".$code." pada aplikasi Suroboyo Maju. Mohon tidak menginformasikan nomor ini kepada siapa pun";
            $api_url=$url."&numbers=".$args["number"]."&content=".rawurlencode($message);
            $ch= curl_init();
            curl_setopt($ch, CURLOPT_URL, $api_url);
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            curl_setopt($ch, CURLOPT_HTTPHEADER, array(
                'Content-Type:application/json',
                'Accept:application/json'
            ));
            curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, FALSE);
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
            curl_setopt($ch, CURLOPT_HEADER, FALSE);
            $res = curl_exec($ch);
            $httpCode= curl_getinfo($ch, CURLINFO_HTTP_CODE);
            curl_close($ch);
            $json = json_decode(utf8_encode($res), true); 
            return $response->withJson($res);
            // if($json["status"]==0){
            //     $new_date = (new DateTime())->modify('+5 minutes');
            //     $expiredToken = $new_date->format('Y/m/d H:i:s'); 
            //     $sql = "UPDATE user set otp_code=:otp_code,otp_code_available_until=:otp_code_available_until where telpon_user=:telpon_user";
            //     $stmt = $this->db->prepare($sql);
            //     $data = [
            //         ":otp_code_available_until"=>$expiredToken,
            //         ":otp_code" => $code,
            //         ":telpon_user"=>$args["number"]
            //     ];
            //     if($stmt->execute($data)){
            //         return $response->withJson(["status" => "1","message"=>"Kode OTP telah dikirimkan ke nomor anda"]);
            //     }
            // }else{
            //     return $response->withJson(["status" => "400","message"=>"Gagal mengirimkan kode OTP, silahkan coba beberapa saat lagi"]);
            // }
        });

        $app->put('/changePassword', function($request,$response){
            $body = $request->getParsedBody();
            $id_user=$body["id_user"];
            $old_password=$body["old_password"];
            $new_password=$body["new_password"];
            $sql="SELECT password_user FROM user where id_user=:id_user";
            $stmt=$this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result=$stmt->fetchColumn();
            if(password_verify($old_password,$result)){
                $sql="UPDATE user SET password_user=:new_password WHERE id_user=:id_user";
                $stmt=$this->db->prepare($sql);
                $data=[
                    ":id_user"=>$id_user,
                    ":new_password"=>password_hash($new_password, PASSWORD_BCRYPT)
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["status"=>"1","message"=>"Password berhasil diubah"]);
                }else{
                    return $response->withJson(["status"=>"400","message"=>"Password tidak berhasil diubah, silahkan coba beberapa saat lagi"]);
                }
                
            }else{
                return $response->withJson(["status"=>"99","message"=>"Password lama tidak sesuai"]);
            }
        });
        
        $app->post('/verifyOTP', function($request,$response){
            $body = $request->getParsedBody();
            $date = new DateTime();
            $sql="SELECT otp_code,otp_code_available_until from user where telpon_user='".$body["number"]."'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            if($result[0]["otp_code"]==$body["otp_code"]){
                if($date<new DateTime($result[0]["otp_code_available_until"])){
                    $sql="UPDATE user set status_aktif_user=1,otp_code=null,otp_code_available_until=null where telpon_user='".$body["number"]."'";
                    $stmt = $this->db->prepare($sql);
                    $stmt->execute();
                    return $response->withJson(["status"=>"1","message"=>"Verify kode OTP berhasil"]);
                }else{
                    return $response->withJson(["status"=>"2","message"=>"Kode OTP telah Kadaluarsa, silahkan request kode OTP yang baru"]);
                }
            }else{
                return $response->withJson(["status"=>"99","message"=>"Kode OTP yang anda masukkan tidak sesuai"]);
            }
        });

        $app->get('/getUser/{id}', function ($request, $response,$args) {
            $id=$args["id"];
            $sql = "SELECT id_user,nama_user,telpon_user,status_user,premium_available_until,lokasi_aktif_user,id_kecamatan_user,status_aktif_user FROM user where id_user=:id";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id" => $id]);
            $result = $stmt->fetch();
            return $response->withJson($result);
        });

        $app->get('/getHistoryLaporanLostFound/{id_user}', function ($request, $response,$args) {
            $id_user=$args["id_user"];
            $sql="SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,skl.nama_kategori AS jenis_barang,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,lf.status_laporan,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                    JOIN user u ON lf.id_user_pelapor=u.id_user 
                    LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                    JOIN setting_kategori_lostfound skl on skl.id_kategori=lf.id_kategori_barang
                    WHERE lf.id_user_pelapor=:id_user
                    GROUP BY lf.id_laporan 
                    ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getHistoryLaporanKriminalitas/{id_user}', function ($request, $response,$args) {
            $id_user=$args["id_user"];
            $sql="SELECT lk.id_laporan,lk.judul_laporan,skk.nama_kategori AS jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,lk.status_laporan,u.nama_user AS nama_user_pelapor, COUNT(kl.id_laporan) AS jumlah_komentar,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lk.id_laporan=kl.id_laporan
                JOIN setting_kategori_kriminalitas skk on skk.id_kategori=lk.id_kategori_kejadian
                WHERE lk.id_user_pelapor=:id_user
                GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->post('/registerCard/{id_user}', function ($request, $response,$args) {
            $body = $request->getParsedBody();
            $card_number=$body["card_number"];
            $card_exp_month=$body["card_exp_month"];
            $card_exp_year=$body["card_exp_year"];
            $client_key = "SB-Mid-client-J4xpVwGv_HmNID-g";
            $API_URL="https://api.sandbox.midtrans.com/v2/card/register?card_number=".$card_number."&card_exp_month=".$card_exp_month."&card_exp_year=".$card_exp_year."&client_key=".$client_key;
            $ch = curl_init(); 
            curl_setopt($ch, CURLOPT_URL, $API_URL); 
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
            curl_setopt($ch,CURLOPT_HTTPHEADER,array (
                "Accept: application/json",
                "Authorization: Basic U0ItTWlkLXNlcnZlci1GQjRNSERieVhlcFc5OFNRWjY0SHhNeEU="
            ));
            $curl_response = curl_exec($ch);  
            $json = json_decode(utf8_encode($curl_response), true); 
            curl_close($ch);
            if($json["status_code"]=="200"){
                $sql = "UPDATE user set credit_card_token=:credit_card_token where id_user=:id_user";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":credit_card_token" => $json["saved_token_id"],
                    ":id_user"=>$args["id_user"]
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["status" => "1"]);
                }
            }else{
                return $response->withJson(["status"=>"400","message"=>"Nomor kartu tidak valid"]);
            }
        });

        $app->put('/updateCreditCardToken', function ($request, $response) {
            $body = $request->getParsedBody();
            $sql = "UPDATE user set credit_card_token=:credit_card_token where id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":credit_card_token" => $body["credit_card_token"],
                ":id_user"=>$body["id_user"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1"]);
            }else{
                return $response->withJson(["status" => "400"]);
            }
        });

        $app->post('/chargeUser/{id_user}', function ($request, $response,$args) {
            $body = $request->getParsedBody();
            $id_user=$args["id_user"];
            $sql = "SELECT * FROM user where id_user='$id_user'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $row = $stmt->fetch();
            $credit_card_token = $row["credit_card_token"];    
            $milliseconds = round(microtime(true) * 1000);
            $id_order="ORDER".$milliseconds;
            $curl = curl_init();
            curl_setopt_array($curl, array(
                CURLOPT_URL => "https://api.sandbox.midtrans.com/v2/charge",
                CURLOPT_RETURNTRANSFER => true,
                CURLOPT_ENCODING => "",
                CURLOPT_MAXREDIRS => 10,
                CURLOPT_TIMEOUT => 0,
                CURLOPT_FOLLOWLOCATION => true,
                CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
                CURLOPT_CUSTOMREQUEST => "POST",
                CURLOPT_POSTFIELDS =>  json_encode([
                    "payment_type" => "credit_card",
                    "transaction_details" => [
                        "order_id" => $id_order,
                        "gross_amount" => 50000
                    ],
                    "credit_card" => [
                        "token_id" => $credit_card_token,
                    ],
                    "item_details" => [[
                        "id" => "SSMEMBER01",
                        "price" => 50000,
                        "quantity" => 1,
                        "name" => "Membership Sahabat Surabaya 1 Bulan",
                        "merchant_name" => "Sahabat Surabaya"
                    ]],
                    "customer_details" => [
                        "first_name" => $row["nama_user"],
                        "email" => $row["email_user"],
                        "phone" => $row["telpon_user"],
                    ]
                ]),
                CURLOPT_HTTPHEADER => array(
                    "Content-Type: application/json",
                    "Accept: application/json",
                    "Authorization: Basic U0ItTWlkLXNlcnZlci1GQjRNSERieVhlcFc5OFNRWjY0SHhNeEU="
                ),
            ));
            $curl_response = curl_exec($curl);  
            $json = json_decode(utf8_encode($curl_response), true); 
            curl_close($curl);
            if($json["status_code"]=="200"){
                $date=$body["tanggal_charge"];
                $sql = "INSERT INTO order_subscription(id_order,order_ammount, order_date,id_user) VALUE (:id_order,:order_ammount,:order_date,:id_user)";
                $data = [
                    ":id_order" => $id_order,
                    ":order_ammount"=>50000,
                    ":order_date" => $date,
                    ":id_user"=>$id_user
                ];
                $stmt=$this->db->prepare($sql);
                $stmt->execute($data);
                $available_until = strtotime($date);
                $final = date("Y-m-d", strtotime("+1 month", $available_until));
                $sql="UPDATE user set premium_available_until=:premium_available_until,status_user=1 where id_user=:id_user";
                $data=[
                  ":premium_available_until"=> $final,
                  ":id_user"=>$id_user
                ];
                $stmt=$this->db->prepare($sql);
                if($stmt->execute($data)){
                    return $response->withJson(["status"=>"1","message"=>"Berhasil melakukan penagihan","premium_available_until"=>$final]); 
                }else{
                    return $response->withJson(["status"=>"99","message"=>"Proses penagihan bermasalah, silahkan coba beberapa saat lagi"]); 
                }
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal melakukan penagihan"]); 
            }
        });

        $app->get('/getEmergencyContact/{id_user}', function($request, $response, $args){
            $id_user=$args["id_user"];
            $sql="SELECT * from user where id_user in (SELECT case when id_user_1=:id_user then id_user_2 else id_user_1 end from daftar_kontak_darurat where status_relasi=1 and (id_user_1=:id_user or id_user_2=:id_user));";
            $data=[
                ":id_user"=>$id_user
            ];
            $stmt = $this->db->prepare($sql);
            $stmt->execute($data);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        }); 

        $app->get('/getSentPendingContactRequest/{id_user}', function($request, $response, $args){
            $id_user=$args["id_user"];
            $sql="SELECT * from user where id_user in (SELECT id_user_2 FROM daftar_kontak_darurat where id_user_1=".$id_user." and status_relasi=0)";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        }); 
        
        $app->get('/getPendingContactRequest/{id_user}', function($request, $response, $args){
            $id_user=$args["id_user"];
            $sql="SELECT dkd.id_daftar_kontak, u.id_user,u.nama_user,u.telpon_user from daftar_kontak_darurat dkd, user u where dkd.id_user_2=".$id_user." and dkd.status_relasi=0 and dkd.id_user_1=u.id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        }); 
        
        $app->post('/addEmergencyContact', function($request, $response){
            $body = $request->getParsedBody();
            $id_user_pengirim=$body["id_user_pengirim"];
            $sql  = "SELECT * from user where telpon_user='".$body["nomor_tujuan"]."'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $user_found = $stmt->fetchAll();
            if($user_found!=null){
                $sql="SELECT * from daftar_kontak_darurat where (id_user_1=:id_user_1 and id_user_2=:id_user_2) OR (id_user_1=:id_user_2 and id_user_2=:id_user_1)";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":id_user_1" => $id_user_pengirim,
                    ":id_user_2"=>$user_found[0]["id_user"]
                ];
                $stmt->execute($data);
                $duplicateRecord = $stmt->fetchAll();
                if($duplicateRecord==null){
                    $sql="INSERT INTO daftar_kontak_darurat (id_user_1,id_user_2,status_relasi) VALUE (:id_user_1,:id_user_2,:status_relasi)";
                    $data = [
                        ":id_user_1" => $id_user_pengirim,
                        ":id_user_2"=>$user_found[0]["id_user"],
                        ":status_relasi"=>0
                    ];
                    $stmt = $this->db->prepare($sql);
                    if($stmt->execute($data)){
                        return $response->withJson(["status"=>"1","message"=>"Request kontak telah dikirimkan ke nomor ".$body["nomor_tujuan"]]);
                    }else{
                        return $response->withJson(["status"=>"400","message"=>"Tambah kontak gagal, silahkan coba beberapa saat lagi"]);
                    }
                }else{
                    $message="";
                    if($duplicateRecord[0]["status_relasi"]==0 && $duplicateRecord[0]["id_user_1"]==$id_user_pengirim){
                        $message="Anda sudah mengirimkan request untuk menambahkan user tersebut sebelumnya, silahkan tunggu user tersebut untuk menerima.";
                    }else if($duplicateRecord[0]["status_relasi"]==0 && $duplicateRecord[0]["id_user_2"]==$id_user_pengirim){
                        $message="User tersebut telah mengirimkan request untuk menambahkan anda kedalam kontaknya.";
                    }else{
                        $message="User dengan nomor tersebut telah terdaftar pada kontak anda.";
                    }
                    return $response->withJson(["status"=>"99","message"=>$message]);
                }       
            }else{
                return $response->withJson(["status"=>"99","message"=>"User dengan nomor tersebut tidak ditemukan"]);
            }
        });

        $app->put('/acceptContactRequest/{id_daftar_kontak}', function ($request, $response,$args) {
            $id_daftar_kontak=$args["id_daftar_kontak"];
            $sql="UPDATE daftar_kontak_darurat SET status_relasi=1 where id_daftar_kontak=".$id_daftar_kontak;
            $stmt = $this->db->prepare($sql);
            if($stmt->execute()){
                return $response->withJson(["status"=>"1","message"=>"Kontak berhasil ditambahkan"]);
            }else{
                return $response->withJson(["status"=>"99","message"=>"Gagal menambahkan kontak, silahkan coba beberapa saat lagi"]);
            }
        });   

        $app->delete('/declineContactRequest/{id_daftar_kontak}', function ($request, $response, $args) {
            $id_daftar_kontak=$args["id_daftar_kontak"];
            $sql="DELETE from daftar_kontak_darurat where id_daftar_kontak=".$id_daftar_kontak;
            $stmt = $this->db->prepare($sql);
            if($stmt->execute()){
                return $response->withJson(["status"=>"1"]);
            }else{
                return $response->withJson(["status"=>"99"]);
            }
        });

        $app->post('/sendEmergencyNotification', function($request, $response){
            $body=$request->getParsedBody();
            $number=$body["number"];
            $heading=$body["heading"];
            $message=$body["content"];
            $id_chat=$body["id_chat"];
            $id_user_pengirim=$body["id_user_pengirim"];
            $id_user_penerima=$body["id_user_penerima"];
            $nama_display=$body["nama_display"];
            $curl = curl_init();
            $content = array(
                "en" => $message
            );
            $heading = array(
                "en" => $heading 
            );
            $data=[
                "page"=>"2",
                "id_chat"=>$id_chat,
                "id_user_pengirim"=>$id_user_pengirim,
                "id_user_penerima"=>$id_user_penerima,
                "nama_display"=>$nama_display
            ];
            sendOneSignalNotification($number,$content,$heading,$data);
            return $response->withJson(["status"=>"1"]);
        });

        $app->get('/checkHeaderChat', function ($request, $response,$args) {   
            $id_user_1=$request->getQueryParam('id_user_1');
            $id_user_2=$request->getQueryParam('id_user_2');
            $sql = "SELECT id_chat from header_chat where id_user_1=:id_user_1 and id_user_2=:id_user_2 or id_user_1=:id_user_2 and id_user_2=:id_user_1";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_user_1" => $id_user_1,
                ":id_user_2" => $id_user_2
            ];
            $stmt->execute($data);
            $result = $stmt->fetchColumn();
            if($result==false){
                $sql = "INSERT INTO header_chat (id_user_1,id_user_2)VALUE(:id_user_1,:id_user_2)";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":id_user_1" => $id_user_1,
                    ":id_user_2"=>$id_user_2
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["id_chat"=>$this->db->lastInsertId()]);
                }else{
                    return $response->withJson(400);
                }
            }else{
                return $response->withJson(["id_chat"=>$result]);
            }
        });

        $app->post('/insertKomentarLaporan', function ($request, $response) {
            $new_komentar = $request->getParsedBody();
            $datetime = date("Y/m/d H:i:s");
            $sql = "INSERT INTO komentar_laporan(id_laporan,isi_komentar, waktu_komentar,id_user_komentar) VALUE (:id_laporan,:isi_komentar, :waktu_komentar, :id_user_komentar)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $new_komentar["id_laporan"],
                ":isi_komentar"=>$new_komentar["isi_komentar"],
                ":waktu_komentar" => $datetime,
                ":id_user_komentar" => $new_komentar["id_user_komentar"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1", "message" => "Berhasil menambah komentar"], 200);
            }else{
                return $response->withJson(["status" => "400", "message" => "Gagal menambah komentar"], 200);
            } 
        });

        $app->get('/getJumlahKonfirmasiLaporan/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args["id_laporan"];
            $sql="SELECT COUNT(*) FROM konfirmasi_laporan_kriminalitas WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan"=>$id_laporan]);
            $result = $stmt->fetchColumn();
            return $response->withJson(["status"=>"1","count"=>$result]);
        });

        $app->get('/getHeaderChat/{id_user}', function ($request, $response,$args) {   
            $id_user=$args["id_user"];
            $sql = "SELECT h.id_chat,h.id_user_1,h.id_user_2,u.nama_user as nama_user_1,u2.nama_user as nama_user_2 from header_chat h,user u, user u2 where h.id_user_1=u.id_user and h.id_user_2=u2.id_user and (h.id_user_1=:id_user OR h.id_user_2=:id_user)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_user" => $id_user
            ];
            $stmt->execute($data);
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });

        $app->get('/getAllChat/{id_chat}', function ($request, $response,$args) {   
            $id_chat=$args["id_chat"];
            $sql = "SELECT * from detail_chat where id_chat=:id_chat";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_chat" => $id_chat
            ];
            $stmt->execute($data);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getLastMessage/{id_chat}', function ($request, $response,$args) {   
            $id_chat=$args["id_chat"];
            $sql = "SELECT d.isi_chat,d.waktu_chat,u1.nama_user as nama_user_pengirim,u2.nama_user as nama_user_penerima FROM detail_chat d,user u1,user u2 where d.id_chat=:id_chat and d.id_user_pengirim=u1.id_user and d.id_user_penerima=u2.id_user order by d.waktu_chat desc LIMIT 1";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_chat" => $id_chat
            ];
            $stmt->execute($data);
            $result = $stmt->fetch();
            if($result==null){
                return $response->withJson(["status"=>"400"]);
            }else{
                return $response->withJson(["status"=>"1","data"=>$result]);
            }
            
        });

        $app->post('/insertHeaderChat', function ($request, $response) {
            $body = $request->getParsedBody();
            $sql = "INSERT INTO header_chat (id_user_1,id_user_2)VALUE(:id_user_1,:id_user_2)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_user_1" => $body["id_user_1"],
                ":id_user_2"=>$body["id_user_2"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(200);
            }else{
                return $response->withJson(400);
            }
        });
        $app->post('/insertDetailChat', function ($request, $response) {
            $new_chat = $request->getParsedBody();
            $datetime = date("Y/m/d H:i:s");
            $sql = "INSERT INTO detail_chat (id_chat,id_user_pengirim, id_user_penerima, isi_chat, waktu_chat) VALUE (:id_chat,:id_user_pengirim,:id_user_penerima, :isi_chat, :waktu_chat)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_chat"=>$new_chat["id_chat"],
                ":id_user_pengirim" => $new_chat["id_user_pengirim"],
                ":id_user_penerima"=>$new_chat["id_user_penerima"],
                ":isi_chat" => $new_chat["isi_chat"],
                ":waktu_chat" => $datetime
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1"]);
            }else{
                return $response->withJson(["status" => "400"]);
            }
        });
    })->add(function ($request, $response, $next) {
        $headers = $request->getHeader("Authorization");
        if($headers!=null){
            $result = Token::validate($headers[0], $_ENV['JWT_SECRET']);
            if($result==false){
                return $response->withJson(["status"=>"400","message"=>"Token not valid"]);
            }else{
                $response = $next($request, $response);
                return $response;
            }         
        }else{
            return $response->withJson(["status"=>"404","message"=>"Token not found"]);
        }
    });
    
        $app->get('/[{name}]', function (Request     $request, Response $response, array $args) use ($container) {
            // Sample log message
            $container->get('logger')->info("Slim-Skeleton '/' route");

            // Render index view
            return $container->get('renderer')->render($response, 'index.phtml', $args);
        });
};
