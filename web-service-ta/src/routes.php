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
    return $json["results"][0]["address_components"][0]["short_name"];
}
return function (App $app) {
    $container = $app->getContainer();
    $container['upload_directory'] = __DIR__ . '/uploads';
    $dotenv = Dotenv\Dotenv::createImmutable(__DIR__);
    $dotenv->load();

    $app->get('/getAllKategoriLostFound', function ($request, $response) {
        $sql = "SELECT * FROM setting_kategori_lostfound";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    });

    $app->post('/sessionSignIn', function ($request, $response) {
        $body=$request->getParsedBody();
        $result = Token::validate($body["token"], $_ENV['JWT_SECRET']);
        if($result==true){
            $payload=Token::getPayload($body["token"], $_ENV['JWT_SECRET']);
            $sql="SELECT * FROM user where id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $payload["sub"]]);
            $user=$stmt->fetch();
            return $response->withJson(["status"=>"1","message"=>"Token verified","data"=>$user]);
        }else{
            return $response->withJson(["status"=>"400","message"=>"Token not verified"]);
        }
    });

    // $app->get('/getHeadlineLaporanLostFound', function ($request, $response) {
    //     $sql = "SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user as nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,gf.nama_file AS thumbnail_gambar FROM laporan_lostfound_barang lf 
    //             JOIN user u ON lf.id_user_pelapor=u.id_user 
    //             LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
    //             JOIN gambar_lostfound_barang gf ON lf.id_laporan=gf.id_laporan
    //             GROUP BY lf.id_laporan 
    //             ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC LIMIT 5";
    //     $stmt = $this->db->prepare($sql);
    //     $stmt->execute();
    //     $result = $stmt->fetchAll();
    //     return $response->withJson($result);
    // });

    $app->get('/getHeadlineLaporanLostFound', function ($request, $response) {
        $sql = "SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                JOIN user u ON lf.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                GROUP BY lf.id_laporan 
                ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC LIMIT 5";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result);
    });

    $app->get('/getLaporanLostFound', function ($request, $response) {
        $sql = "SELECT lf.id_laporan,lf.judul_laporan,lf.jenis_laporan,lf.tanggal_laporan,lf.waktu_laporan,lf.alamat_laporan,lf.lat_laporan,lf.lng_laporan,lf.deskripsi_barang,lf.deskripsi_barang,lf.id_user_pelapor,u.nama_user AS nama_user_pelapor,count(kl.id_laporan) AS jumlah_komentar,lf.thumbnail_gambar AS thumbnail_gambar FROM laporan_lostfound_barang lf 
                JOIN user u ON lf.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lf.id_laporan=kl.id_laporan
                GROUP BY lf.id_laporan 
                ORDER BY lf.tanggal_laporan DESC, lf.waktu_laporan DESC";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result);
    });

    $app->get('/getLaporanKriminalitas', function ($request, $response) {
        $sql = "SELECT lk.id_laporan,lk.judul_laporan,lk.jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor, COUNT(kl.id_laporan) AS jumlah_komentar,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lk.id_laporan=kl.id_laporan 
                GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result);
    });
    

    // $app->get('/getHeadlineLaporanKriminalitas', function ($request, $response) {
    //     $sql = "SELECT lk.id_laporan,lk.judul_laporan,lk.jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor, COUNT(kl.id_laporan) AS jumlah_komentar,gk.nama_file AS thumbnail_gambar FROM user u 
    //             JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
    //             LEFT JOIN komentar_laporan kl ON lk.id_laporan=kl.id_laporan 
    //             JOIN gambar_kriminalitas gk ON lk.id_laporan=gk.id_laporan
    //             GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC LIMIT 5";
    //     $stmt = $this->db->prepare($sql);
    //     $stmt->execute();
    //     $result = $stmt->fetchAll();
    //     return $response->withJson($result);
    // });

    $app->get('/getHeadlineLaporanKriminalitas', function ($request, $response) {
        $sql = "SELECT lk.id_laporan,lk.judul_laporan,lk.jenis_kejadian,lk.deskripsi_kejadian,lk.tanggal_laporan,lk.waktu_laporan,lk.alamat_laporan,lk.lat_laporan,lk.lng_laporan,lk.id_user_pelapor,u.nama_user AS nama_user_pelapor, COUNT(kl.id_laporan) AS jumlah_komentar,lk.thumbnail_gambar AS thumbnail_gambar FROM user u 
                JOIN laporan_kriminalitas lk ON lk.id_user_pelapor=u.id_user 
                LEFT JOIN komentar_laporan kl ON lk.id_laporan=kl.id_laporan 
                GROUP BY lk.id_laporan ORDER BY lk.tanggal_laporan DESC, lk.waktu_laporan DESC LIMIT 5";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result);
    });
    
    $app->get('/getAllKategoriCrime', function ($request, $response) {
        $sql = "SELECT * FROM setting_kategori_kriminalitas";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    }); 
    

    $app->get('/getAllKantorPolisi', function ($request, $response) {   
        $sql = "SELECT * FROM kantor_polisi";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    });
    
    $app->group('/admin', function() use($app){
        $app->get('/getAllUser', function ($request, $response) {
            $sql = "SELECT id_user,telpon_user,nama_user,status_user,status_aktif_user FROM user where status_user!=2";
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
            return $response->withJson($result, 200);
        });
        
        $app->get('/getLaporanLostFoundVerify',function ($request,$response){
            $sql="SELECT id_laporan,judul_laporan,jenis_laporan,jenis_barang,tanggal_laporan,waktu_laporan,alamat_laporan,kecamatan FROM laporan_lostfound_barang";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });

        $app->get('/getDetailLaporanLostFound/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args['id_laporan'];
            $sql="SELECT * FROM laporan_lostfound_barang WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetch();
            return $response->withJson($result, 200);
        });

        $app->get('/getDetailLaporanKriminalitas/{id_laporan}',function ($request,$response,$args){
            $id_laporan=$args['id_laporan'];
            $sql="SELECT * FROM laporan_kriminalitas WHERE id_laporan=:id_laporan";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });

        $app->get('/getLaporanKriminalitasVerify',function ($request,$response){
            $sql="SELECT id_laporan,judul_laporan,jenis_laporan,tanggal_laporan,waktu_laporan,alamat_laporan,kecamatan FROM laporan_lostfound_barang";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });
        
        $app->get('/getKepalaKeamanan   ',function ($request,$response){
            $sql="SELECT * FROM user where status_user=2";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
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
            $sql="UPDATE user SET nama_user=:nama_user,kecamatan_user=:kecamatan_user WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_user" =>$id_user,
                ":nama_user" => $body["nama_user"],
                ":kecamatan_user"=>$body["kecamatan_user"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status"=>"1","message"=>"Berhasil mengubah data kepala keamanan"]);
            }else{
                return $response->withJson(["status"=>"400","message"=>"Gagal mengubah data kepala keamanan"]);
            }
        });

        $app->post('/addKepalaKeamanan',function ($request,$response){
            $body=$request->getParsedBody();
            $sql="SELECT COUNT(*) from user where telpon_user=:telpon_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":telpon_user" => $body["telpon_user"]]);
            $result=$stmt->fetchColumn();
            if($result==0){
                $sql = "INSERT INTO user (password_user, nama_user, telpon_user, status_user,kecamatan_user) VALUE (:password_user, :nama_user, :telpon_user, :status_user,:kecamatan_user)";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":password_user"=>password_hash($body["password_user"], PASSWORD_BCRYPT),
                    ":nama_user" => $body["nama_user"],
                    ":telpon_user" => $body["telpon_user"],
                    ":status_user"=>2,
                    ":kecamatan_user"=>$body["kecamatan_user"]
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
    });
    
    $app->group('/kepalaKeamanan',function() use($app){
        $app->get('/getLaporanLostFound/{kecamatan}',function($request,$response,$args){
            $kecamatan=$args["kecamatan"];
            $sql="SELECT * FROM laporan_lostfound_barang WHERE kecamatan LIKE '%$kecamatan%'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":kecamatan" => $kecamatan]);
            $result=$stmt->fetchAll();
            return $response->withJson($result);
        });
        $app->get('/cc',function($request,$response){
            $headers = $request->getHeader("Authorization");
            return $headers[0];
        });
    });
    // ->add(function ($request, $response, $next) {
    //     $headers = $request->getHeader("Authorization");
    //     if($headers!=null){
    //         $response = $next($request, $response);
    //         return $response;
    //     }else{
    //         return $response->withJson('No Token');
    //     }
    // });

    $app->group('/user', function () use ($app) {
        $app->put('/updateProfile',function ($request, $response){
            $body=$request->getParsedBody();
            $id_user=$body["id_user"];
            $password_user=$body["password_user"];
            $sql="SELECT password_user FROM user WHERE id_user=:id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result=$stmt->fetchColumn();
            if(password_verify($password_user,$result)){
                $geohash=new Geohash();
                $lat_user=null;
                $lng_user=null;
                $lokasi_aktif_user=null;
                $geohash_lokasi_aktif_user=null;
                if($body["lokasi_available"]=="1"){
                    $lat_user=$body["lat_user"];
                    $lng_user=$body["lng_user"];
                    $lokasi_aktif_user=$body["lokasi_aktif_user"];
                    $geohash_lokasi_aktif_user=$geohash->encode(floatval($lat_user), floatval($lng_user), 8);
                }
                $sql="UPDATE user SET nama_user=:nama_user, lat_user=:lat_user, lng_user=:lng_user, lokasi_aktif_user=:lokasi_aktif_user, geohash_lokasi_aktif_user=:geohash_lokasi_aktif_user WHERE id_user=:id_user";
                $stmt=$this->db->prepare($sql); 
                $data=[
                    ":id_user"=>$id_user,
                    ":nama_user"=>$body["nama_user"],
                    ":lat_user"=>$lat_user,
                    ":lng_user"=>$lng_user,
                    ":lokasi_aktif_user"=>$lokasi_aktif_user,
                    ":geohash_lokasi_aktif_user"=>$geohash_lokasi_aktif_user
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

        $app->post('/sendOTP', function($request,$response){
            $body = $request->getParsedBody();
            $url = "https://numberic1.tcastsms.net:20005/sendsms?account=def_robby3&password=123456";
            $code=rand(1000,9999);
            $message="Masukkan nomor ".$code.". Mohon tidak menginformasikan nomor ini kepada siapa pun";
            $api_url=$url."&numbers=".$body["number"]."&content=".rawurlencode($message);
            // $ch= curl_init();
            // curl_setopt($ch, CURLOPT_URL, $api_url);
            // curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
            // curl_setopt($ch, CURLOPT_HTTPHEADER, array(
            //     'Content-Type:application/json',
            //     'Accept:application/json'
            // ));
            // curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, FALSE);
            // curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
            // curl_setopt($ch, CURLOPT_HEADER        , FALSE);
    
            // $res = curl_exec($ch);
            // $httpCode= curl_getinfo($ch, CURLINFO_HTTP_CODE);
            // curl_close($ch);

            $new_date = (new DateTime())->modify('+5 minutes');
            $expiredToken = $new_date->format('Y/m/d H:i:s'); 
            $sql = "UPDATE user set otp_code=:otp_code,otp_code_available_until=:otp_code_available_until where telpon_user=:telpon_user";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":otp_code_available_until"=>$expiredToken,
                ":otp_code" => $code,
                ":telpon_user"=>$body["number"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status_code" => "200"]);
            }else{
                return $response->withJson(["status_code" => "400"]);
            }
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

        $app->post('/checkLogin', function ($request, $response) {
            $body = $request->getParsedBody();
            $sql = "SELECT * FROM user where telpon_user='".$body["telpon_user"]."'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetch();
            if($result!=null){
                if(password_verify($body["password_user"],$result["password_user"])){
                    $payload = array(
                        "iss" => "slim-framework",
                        "sub" => $result["id_user"],
                        "iat" => time(),
                        "exp" => time()+60*60
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

        $app->get('/getUser/{id}', function ($request, $response,$args) {
            $id=$args["id"];
            $sql = "SELECT id_user,nama_user,telpon_user,status_user,premium_available_until,lokasi_aktif_user,kecamatan_user,status_aktif_user FROM user where id_user=:id";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id" => $id]);
            $result = $stmt->fetch();
            return $response->withJson($result);
        });

        $app->get('/getHistoryLaporanLostFound/{id_user}', function ($request, $response,$args) {
            $id_user=$args["id_user"];
            $sql = "SELECT * FROM laporan_lostfound_barang where id_user_pelapor=:id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
        });

        $app->get('/getHistoryLaporanKriminalitas/{id_user}', function ($request, $response,$args) {
            $id_user=$args["id_user"];
            $sql = "SELECT * FROM laporan_kriminalitas where id_user_pelapor=:id_user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_user" => $id_user]);
            $result = $stmt->fetchAll();
            return $response->withJson($result);
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
                return $response->withJson(["status" => "success", "data" => "1"], 200);
            }else{
                return $response->withJson(["status" => "failed", "data" => "0"], 200);
            }
        });

        $app->post('/chargeUser', function ($request, $response) {
            $body = $request->getParsedBody();
            $id_user=$body["id_user"];
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
                 $datetime=date('Y-m-d');
                $sql = "INSERT INTO order_subscription(id_order,order_ammount, order_date) VALUE (:id_order,:order_ammount,:order_date)";
                $data = [
                    ":id_order" => $id_order,
                    ":order_ammount"=>50000,
                    ":order_date" => $datetime
                ];
                $stmt=$this->db->prepare($sql);
                $stmt->execute($data);
                $available_until = strtotime($datetime);
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

       $app->post('/registerUser', function ($request, $response) {
            $new_user = $request->getParsedBody();
            $sql="SELECT COUNT(*) from user where telpon_user='".$new_user["telpon_user"]."'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $telpon_kembar = $stmt->fetchColumn();
            if($telpon_kembar==1){
                return $response->withJson(["status"=>"400","message"=>"No. Handphone yang dimasukkan telah terpakai"]);
            }else{
                $geohash=new Geohash();
                $alamat_available=$new_user["alamat_available"];
                $lat_user=null;
                $lng_user=null;
                $lokasi_aktif_user=null; 
                $geohash_lokasi_aktif_user=null;        
                if($alamat_available=="1"){
                    $lat_user=$new_user["lat_user"];
                    $lng_user=$new_user["lng_user"];
                    $lokasi_aktif_user=$new_user["lokasi_aktif_user"];
                    $geohash_lokasi_aktif_user=$geohash->encode(floatval($lat_user), floatval($lng_user), 8);
                }
                $sql = "INSERT INTO user (telpon_user, password_user, nama_user, status_user, lat_user,lng_user,lokasi_aktif_user,geohash_lokasi_aktif_user,status_aktif_user) VALUE (:telpon_user, :password_user, :nama_user, :status_user, :lat_user,:lng_user,:lokasi_aktif_user,:geohash_lokasi_aktif_user,:status_aktif_user)";
                $stmt = $this->db->prepare($sql);
                $data = [
                    ":password_user"=>password_hash($new_user["password_user"], PASSWORD_BCRYPT),
                    ":nama_user" => $new_user["nama_user"],
                    ":telpon_user" => $new_user["telpon_user"],
                    ":status_user"=>0,
                    ":lat_user"=>$lat_user,
                    ":lng_user"=>$lng_user,
                    ":lokasi_aktif_user"=>$lokasi_aktif_user,
                    ":geohash_lokasi_aktif_user"=>$geohash_lokasi_aktif_user,
                    ":status_aktif_user"=>99
                ];
                if($stmt->execute($data)){
                    return $response->withJson(["status" => "1","message"=>"Register akun berhasil!","insertID"=>$this->db->lastInsertId()]);
                }else{
                    return $response->withJson(["status" => "99","message"=>"Register gagal, silahkan coba beberapa saat lagi"]);
                }
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
            //$sql="SELECT * from user where id_user in (SELECT id_user_1 FROM daftar_kontak_darurat where id_user_2=".$id_user." and status_relasi=0)";
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
            $sql="UPDATE daftar_kontak_darurat set status_relasi=1 where id_daftar_kontak=".$id_daftar_kontak;
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
            $curl = curl_init();
            $content = array(
                "en" => $content
            );
            $heading = array(
                "en" => $heading 
            );
            $fields = array(
                'app_id' => "6fd226ba-1d41-4c7b-9f8b-a973a8fd436b",
                'filters' => array(array("field" => "tag", "key" => "no_handphone", "relation" => "=", "value" => $number)),
                'contents' => $content,
                'headings' => $heading
            );
            $fields = json_encode($fields);
            $ch = curl_init();
            curl_setopt($ch, CURLOPT_URL, "https://onesignal.com/api/v1/notifications");
            curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json; charset=utf-8',
                                                    'Authorization: Basic MDUyNjhlOGEtNDQ4NC00YTYwLWIxYmYtMDZjYTc2OGUwNDc4'));
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
            curl_setopt($ch, CURLOPT_HEADER, FALSE);
            curl_setopt($ch, CURLOPT_POST, TRUE);
            curl_setopt($ch, CURLOPT_POSTFIELDS, $fields);
            curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
            $res = curl_exec($ch);
            curl_close($ch);
            return $response->withJson($res);
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
            $datetime = DateTime::createFromFormat('d/m/Y', $new_komentar["tanggal_komentar"]);
            $day=$datetime->format('d');
            $month=$datetime->format('m');
            $year=$datetime->format('Y');
            $formatDate=$year.$month.$day;
            $sql = "INSERT INTO komentar_laporan(id_laporan,isi_komentar, tanggal_komentar, waktu_komentar,id_user_komentar) VALUE (:id_laporan,:isi_komentar, :tanggal_komentar, :waktu_komentar, :id_user_komentar)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $new_komentar["id_laporan"],
                ":isi_komentar"=>$new_komentar["isi_komentar"],
                ":tanggal_komentar" => $formatDate,
                ":waktu_komentar" => $new_komentar["waktu_komentar"],
                ":id_user_komentar" => $new_komentar["id_user_komentar"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1", "message" => "Berhasil menambah komentar"], 200);
            }else{
                return $response->withJson(["status" => "400", "message" => "Gagal menambah komentar"], 200);
            } 
        });

        $app->post('/konfirmasiLaporanKriminalitas',function ($request,$response){
            $body = $request->getParsedBody();
            $sql="INSERT INTO konfirmasi_laporan_kriminalitas VALUES(:id_laporan,:id_user)";
            $stmt = $this->db->prepare($sql);
            $data=[
                ":id_laporan"=>$body["id_laporan"],
                ":id_user"=>$body["id_user"]
            ];
            if($stmt->execute($data)){
                return $response->withJson(["status" => "1", "message" => "Konfirmasi Laporan Berhasil"], 200);
            }else{
                return $response->withJson(["status" => "400", "message" => "Konfirmasi Laporan Gagal"], 200);
            }
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
            $datetime = date("Y/m/d H:i");
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
    });
        
        $app->post('/insertLaporanLostFound', function(Request $request, Response $response,$args) {
            $new_laporan = $request->getParsedBody();
            $datetime = DateTime::createFromFormat('d/m/Y', $new_laporan["tanggal_laporan"]);
            $day=$datetime->format('d');
            $month=$datetime->format('m');
            $year=$datetime->format('Y');
            $formatDate=$year.$month.$day;
            $id_laporan="LF".$day.$month.$year;
            $geohash=new Geohash();
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
            $kecamatan=getKecamatan($new_laporan["lat_laporan"],$new_laporan["lng_laporan"]);
            $sql = "INSERT INTO laporan_lostfound_barang VALUES(:id_laporan,:judul_laporan,:jenis_laporan,:jenis_barang,:tanggal_laporan,:waktu_laporan,:alamat_laporan,:lat_laporan,:lng_laporan,:deskripsi_barang,:id_user_pelapor,:status_laporan,:geohash_alamat_laporan,:kecamatan,:thumbnail_gambar) ";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $id_laporan,
                ":judul_laporan"=>$new_laporan["judul_laporan"],
                ":jenis_laporan" => $new_laporan["jenis_laporan"],
                ":jenis_barang"=> $new_laporan["jenis_barang"],
                ":alamat_laporan"=>$new_laporan["alamat_laporan"],
                ":tanggal_laporan"=>$formatDate,
                ":waktu_laporan"=>$new_laporan["waktu_laporan"],
                ":lat_laporan"=>$new_laporan["lat_laporan"],
                ":lng_laporan"=>$new_laporan["lng_laporan"],
                ":deskripsi_barang"=>$new_laporan["deskripsi_barang"],
                ":id_user_pelapor"=>$new_laporan["id_user_pelapor"],
                ":status_laporan"=>0,
                ":geohash_alamat_laporan"=> $geohash->encode(floatval($new_laporan["lat_laporan"]), floatval($new_laporan["lng_laporan"]), 8),
                ":kecamatan"=>$kecamatan,
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
            $new_laporan = $request->getParsedBody();
            $datetime = DateTime::createFromFormat('d/m/Y', $new_laporan["tanggal_laporan"]);
            $day=$datetime->format('d');
            $month=$datetime->format('m');
            $year=$datetime->format('Y');
            $formatDate=$year.$month.$day;
            $id_laporan="CR".$day.$month.$year;
            $geohash=new Geohash();
            $sql="SELECT COUNT(*)+1 from laporan_kriminalitas where id_laporan like'%$id_laporan%'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();     
            $uploadedFiles = $request->getUploadedFiles();
            $id_laporan=$id_laporan.str_pad($result,5,"0",STR_PAD_LEFT);    
            $filename="default.png";
            if($uploadedFiles!=null){
                $uploadedFile = $uploadedFiles['image'];
                $extension = pathinfo($uploadedFile->getClientFilename(), PATHINFO_EXTENSION);
                $filename=$id_laporan.".".$extension;
            }      
            $kecamatan=getKecamatan($new_laporan["lat_laporan"],$new_laporan["lng_laporan"]);
            $sql = "INSERT INTO laporan_kriminalitas VALUES(:id_laporan,:judul_laporan,:jenis_kejadian,:deskripsi_kejadian,:tanggal_laporan,:waktu_laporan,:alamat_laporan,:lat_laporan,:lng_laporan,:id_user_pelapor,:status_laporan,:geohash_alamat_laporan,:kecamatan,:thumbnail_gambar) ";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $id_laporan,
                ":judul_laporan"=>$new_laporan["judul_laporan"],
                ":jenis_kejadian" => $new_laporan["jenis_kejadian"],
                ":deskripsi_kejadian"=>$new_laporan["deskripsi_kejadian"],
                ":tanggal_laporan"=>$formatDate,
                ":waktu_laporan"=>$new_laporan["waktu_laporan"],
                ":alamat_laporan"=>$new_laporan["alamat_laporan"],
                ":lat_laporan"=>$new_laporan["lat_laporan"],
                ":lng_laporan"=>$new_laporan["lng_laporan"],
                ":id_user_pelapor"=>$new_laporan["id_user_pelapor"],
                ":status_laporan"=>0,
                ":geohash_alamat_laporan"=> $geohash->encode(floatval($new_laporan["lat_laporan"]), floatval($new_laporan["lng_laporan"]), 8),
                ":kecamatan"=>$kecamatan,
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

        $app->get('/getKomentarLaporan/{id_laporan}', function ($request, $response,$args) {
            $id_laporan=$args["id_laporan"];
            $sql = "SELECT kl.id_komentar,kl.id_laporan,kl.isi_komentar,kl.tanggal_komentar,kl.waktu_komentar,u.nama_user AS nama_user_komentar
                    FROM komentar_laporan kl, user u 
                    WHERE kl.id_user_komentar=u.id_user and kl.id_laporan=:id_laporan
                    ORDER BY kl.tanggal_komentar DESC, kl.waktu_komentar DESC";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id_laporan" => $id_laporan]);
            $result = $stmt->fetchAll();
            return $response->withJson($result, 200);
        });

        $app->get('/[{name}]', function (Request     $request, Response $response, array $args) use ($container) {
            // Sample log message
            $container->get('logger')->info("Slim-Skeleton '/' route");

            // Render index view
            return $container->get('renderer')->render($response, 'index.phtml', $args);
        });
};
