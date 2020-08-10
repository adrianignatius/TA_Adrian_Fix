<?php

use Slim\App;
use Slim\Http\Request;
use Slim\Http\Response;
use Slim\Http\UploadedFile;

return function (App $app) {
    $container = $app->getContainer();
    $container['upload_directory'] = __DIR__ . '/uploads';

    $app->get('/getAllKategoriLostFound', function ($request, $response) {
        $sql = "SELECT * FROM setting_kategori_lostfound";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    });

    $app->get('/getHeadlineLaporanLostFound', function ($request, $response) {
        $sql = "SELECT * FROM laporan_lostfound_barang order by tanggal_laporan desc, waktu_laporan desc LIMIT 5";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    });

    $app->get('/getHeadlineLaporanKriminalitas', function ($request, $response) {
        $sql = "SELECT * FROM laporan_kriminalitas order by tanggal_laporan desc, waktu_laporan desc LIMIT 5";
        $stmt = $this->db->prepare($sql);
        $stmt->execute();
        $result = $stmt->fetchAll();
        return $response->withJson($result, 200);
    });

    $app->get('/getCoba', function ($request, $response) {
        $arr=[1,2,3];
        return $response->withJson($arr, 200);
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


    $app->group('/user', function () use ($app) {
        $app->get('/getAllUser', function ($request, $response) {
            $sql = "SELECT * FROM user";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchAll();
            return $response->withJson(["status" => "success", "users" => $result], 200);
        });

        $app->post('/checkLogin', function ($request, $response) {
            $user = $request->getParsedBody();
            $email=$user["email"];
            $password=$user["password"];
            $sql = "SELECT count(*) as ada FROM user where email_user='$email' and password_user='$password'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            return $response->withJson(["status" => "success", "data" => $result], 200);
        });

        $app->get('/getUser/{id}', function ($request, $response,$args) {
            $id=$args["id"];
            $sql = "SELECT * FROM user where id_user=:id";
            $stmt = $this->db->prepare($sql);
            $stmt->execute([":id" => $id]);
            $result = $stmt->fetchAll();
            return $response->withJson(["status" => "success", "data" => $result], 200);
        });

       $app->post('/insertUser', function ($request, $response) {
            $new_user = $request->getParsedBody();
            $sql = "INSERT INTO user (email_user,password_user, nama_user, telpon_user) VALUE (:email_user,:password_user, :nama_user, :telpon_user)";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":email_user" => $new_user["email_user"],
                ":password_user"=>$new_user["password_user"],
                ":nama_user" => $new_user["nama_user"],
                ":telpon_user" => $new_user["telpon_user"]
            ];
        
            if($stmt->execute($data))
            return $response->withJson(["status" => "success", "data" => "1"], 200);
            
            return $response->withJson(["status" => "failed", "data" => "0"], 200);
            });
    });
        
    

        $app->post('/insertLaporanLostFound', function(Request $request, Response $response,$args) {
            $new_laporan = $request->getParsedBody();
            $datetime = DateTime::createFromFormat('d/m/Y', $new_laporan["tanggal_laporan"]);
            $day=$datetime->format('d');
            $month=$datetime->format('m');
            $year=$datetime->format('Y');
            $id_laporan="LF".$day.$month.$year;
            $sql="SELECT COUNT(*)+1 from laporan_lostfound_barang where id_laporan like'%$id_laporan%'";
            $stmt = $this->db->prepare($sql);
            $stmt->execute();
            $result = $stmt->fetchColumn();
            $formatDate=$year.$month.$day;
            $id_laporan=$id_laporan.str_pad($result,5,"0",STR_PAD_LEFT);
            $sql = "INSERT INTO laporan_lostfound_barang VALUES(:id_laporan,:judul_laporan,:jenis_laporan,:tanggal_laporan,:waktu_laporan,:alamat_laporan,:lat_laporan,:lng_laporan,:deskripsi_barang,:email_pelapor,:status_laporan) ";
            $stmt = $this->db->prepare($sql);
            $data = [
                ":id_laporan" => $id_laporan,
                ":judul_laporan"=>$new_laporan["judul_laporan"],
                ":jenis_laporan" => $new_laporan["jenis_laporan"],
                ":alamat_laporan"=>$new_laporan["alamat_laporan"],
                ":tanggal_laporan"=>$formatDate,
                ":waktu_laporan"=>$new_laporan["waktu_laporan"],
                ":lat_laporan"=>$new_laporan["lat_laporan"],
                ":lng_laporan"=>$new_laporan["lng_laporan"],
                ":deskripsi_barang"=>$new_laporan["deskripsi_barang"],
                ":email_pelapor"=>$new_laporan["email_pelapor"],
                ":status_laporan"=>0
            ];
            $stmt->execute($data);
            // return $response->withJson(["status" => "success", "data" => "1"], 200);
            // return $response->withJson(["status" => "failed", "data" => "0"], 400);
            $increment=1;
            $uploadedFiles = $request->getUploadedFiles();
            foreach($uploadedFiles['image'] as $uploadedFile){
                if ($uploadedFile->getError() === UPLOAD_ERR_OK) {
                    $id_gambar=$id_laporan.$increment;
                    $extension = pathinfo($uploadedFile->getClientFilename(), PATHINFO_EXTENSION);
                    $filename=$id_gambar.".".$extension;
                    $sql = "INSERT INTO gambar_lostfound_barang VALUES(:id_gambar,:nama_file,:id_laporan) ";
                    $stmt = $this->db->prepare($sql);
                    $data = [
                        ":id_gambar" => $id_gambar,
                        ":nama_file"=>$filename,
                        ":id_laporan" => $id_laporan
                    ];
                    $stmt->execute($data);
                    $directory = $this->get('settings')['upload_directory'];
                    $uploadedFile->moveTo($directory . DIRECTORY_SEPARATOR . $filename); 
                    $increment=$increment+1;
                }
            }
        });

        $app->get('/[{name}]', function (Request $request, Response $response, array $args) use ($container) {
            // Sample log message
            $container->get('logger')->info("Slim-Skeleton '/' route");

            // Render index view
            return $container->get('renderer')->render($response, 'index.phtml', $args);
        });
};
