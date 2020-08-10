-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 30, 2020 at 07:26 AM
-- Server version: 10.4.11-MariaDB
-- PHP Version: 7.4.5

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `sahabat_surabaya`
--
CREATE DATABASE IF NOT EXISTS `sahabat_surabaya` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `sahabat_surabaya`;

-- --------------------------------------------------------

--
-- Table structure for table `gambar_lostfound_barang`
--

DROP TABLE IF EXISTS `gambar_lostfound_barang`;
CREATE TABLE IF NOT EXISTS `gambar_lostfound_barang` (
  `id_gambar` varchar(16) NOT NULL,
  `nama_file` varchar(25) NOT NULL,
  `id_laporan` varchar(15) NOT NULL,
  PRIMARY KEY (`id_gambar`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `gambar_lostfound_barang`
--

INSERT INTO `gambar_lostfound_barang` (`id_gambar`, `nama_file`, `id_laporan`) VALUES
('LF03092020000021', 'LF03092020000021.jpg', 'LF0309202000002'),
('LF03092020000022', 'LF03092020000022.jpg', 'LF0309202000002'),
('LF09052020000011', 'LF09052020000011.jpg', 'LF0905202000001'),
('LF09052020000021', 'LF09052020000021.jpg', 'LF0905202000002'),
('LF09052020000022', 'LF09052020000022.jpg', 'LF0905202000002');

-- --------------------------------------------------------

--
-- Table structure for table `laporan_lostfound_barang`
--

DROP TABLE IF EXISTS `laporan_lostfound_barang`;
CREATE TABLE IF NOT EXISTS `laporan_lostfound_barang` (
  `id_laporan` varchar(15) NOT NULL COMMENT 'autogenerate dengan format "LF+2 digit tanggal + 2 digit bulan +2 digit tahun + 5 digit nomor urut',
  `judul_laporan` varchar(50) NOT NULL,
  `jenis_laporan` int(11) NOT NULL COMMENT '0 untuk penemuan barang\r\n1 untuk kehilangan barang',
  `tanggal_laporan` date NOT NULL,
  `waktu_laporan` time NOT NULL,
  `alamat_laporan` varchar(255) NOT NULL,
  `lat_laporan` varchar(20) NOT NULL,
  `lng_laporan` varchar(20) NOT NULL,
  `deskripsi_barang` varchar(255) NOT NULL,
  `email_pelapor` varchar(50) NOT NULL,
  `status_laporan` int(1) NOT NULL COMMENT '0 untuk belum di verifikasi\r\n1 untuk sudah diverifikasi',
  PRIMARY KEY (`id_laporan`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `laporan_lostfound_barang`
--

INSERT INTO `laporan_lostfound_barang` (`id_laporan`, `judul_laporan`, `jenis_laporan`, `tanggal_laporan`, `waktu_laporan`, `alamat_laporan`, `lat_laporan`, `lng_laporan`, `deskripsi_barang`, `email_pelapor`, `status_laporan`) VALUES
('LF0307202000001', 'asdasd', 0, '2020-07-03', '22:23:46', 'Jl. Ngagel Jaya Tengah 73 Baratajaya', '-7.291135', '112.758665', 'asd', 'asd@xyz.com', 0),
('LF0309202000001', 'asdasd', 1, '2020-09-03', '08:13:54', 'Jl. Manggis X 730 Kec. Waru', '-7.3508824', '112.7860851', 'asdasd', 'asd@xyz.com', 0),
('LF0905202000001', 'asdsadsa', 0, '2020-05-09', '12:44:07', 'Jl. Ngagel Jaya Tengah 73 Baratajaya', '-7.291135', '112.758665', 'asdsa', 'asd@xyz.com', 0),
('LF0905202000002', 'asdsadsa', 0, '2020-05-09', '12:44:07', 'Jl. Ngagel Jaya Tengah 73 Baratajaya', '-7.291135', '112.758665', 'asdsa', 'asd@xyz.com', 0);

-- --------------------------------------------------------

--
-- Table structure for table `setting_kategori_kriminalitas`
--

DROP TABLE IF EXISTS `setting_kategori_kriminalitas`;
CREATE TABLE IF NOT EXISTS `setting_kategori_kriminalitas` (
  `id_kategori` int(11) NOT NULL AUTO_INCREMENT,
  `nama_kategori` varchar(40) NOT NULL,
  `file_gambar_kategori` varchar(255) NOT NULL,
  PRIMARY KEY (`id_kategori`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `setting_kategori_kriminalitas`
--

INSERT INTO `setting_kategori_kriminalitas` (`id_kategori`, `nama_kategori`, `file_gambar_kategori`) VALUES
(1, 'Penculikkan', 'kidnap.jpg'),
(2, 'Pencurian', 'robbery.jpg'),
(3, 'Perusakkan Fasilitas Umum', 'watch.jpg'),
(4, 'Tabrak Lari', 'hitrun.jpg'),
(5, 'Kekerasan Pada Perempuan dan Anak', 'violence.jpg'),
(6, 'Aktifitas Mencurigakan', 'suspicious-activity.jpg'),
(7, 'Lainnya', 'other.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `setting_kategori_lostfound`
--

DROP TABLE IF EXISTS `setting_kategori_lostfound`;
CREATE TABLE IF NOT EXISTS `setting_kategori_lostfound` (
  `id_kategori` int(11) NOT NULL AUTO_INCREMENT,
  `nama_kategori` varchar(25) NOT NULL,
  `file_gambar_kategori` varchar(255) NOT NULL,
  PRIMARY KEY (`id_kategori`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `setting_kategori_lostfound`
--

INSERT INTO `setting_kategori_lostfound` (`id_kategori`, `nama_kategori`, `file_gambar_kategori`) VALUES
(1, 'Handphone', 'handphone-icon.png'),
(2, 'Tas', 'bag-icon.png'),
(3, 'Jam Tangan', 'watch-icon.png'),
(4, 'Perhiasan', 'jewerly-icon.png'),
(5, 'Sepatu', 'shoes-icon.png'),
(6, 'Hewan Peliharaan', 'pet-icon.png');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
CREATE TABLE IF NOT EXISTS `user` (
  `id_user` int(11) NOT NULL AUTO_INCREMENT,
  `email_user` varchar(50) NOT NULL,
  `password_user` varchar(50) NOT NULL,
  `nama_user` varchar(30) NOT NULL,
  `telpon_user` varchar(15) NOT NULL,
  PRIMARY KEY (`id_user`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=latin1;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`id_user`, `email_user`, `password_user`, `nama_user`, `telpon_user`) VALUES
(1, 'adrianignatius27@yahoo.com', 'adr', 'adrian', '081330055037'),
(2, 'budi@gmail.com', 'bud', 'budi', '085123512352'),
(3, 'userbaru@stts.edu', 'usr', 'userbaru', '0851235123'),
(6, 'Samuel@gmail.com', 'samuel', 'Samuel', '0851235123'),
(7, 'asd', 'asd', 'asd', 'aasd'),
(9, 'wqeqwe', 'asdsadqwtqw', 'uqwew', 'zxc'),
(10, 'qwewqe', 'tqwewq', 'yqwewq', 'yqweqw'),
(13, 'x,lzxc,;zl', 'cxz;l', 'poxcpxzk', 'xzcqwpdw'),
(14, 'kk', 'yqwekwqlek', 'asy-qwe', 'yqwpeo');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
