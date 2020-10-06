-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Sep 09, 2020 at 05:55 PM
-- Server version: 10.4.11-MariaDB
-- PHP Version: 7.2.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `laravel_onluck`
--

-- --------------------------------------------------------

--
-- Table structure for table `auth_vendors`
--

CREATE TABLE `auth_vendors` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `profile_picture` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `user_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `vendor_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `auth_vendors`
--

INSERT INTO `auth_vendors` (`id`, `profile_picture`, `user_name`, `vendor_name`, `user_id`, `created_at`, `updated_at`) VALUES
(19, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 14, '2020-08-23 00:07:28', '2020-08-23 00:07:28'),
(20, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 15, '2020-08-23 02:15:01', '2020-08-23 02:15:01'),
(21, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 16, '2020-08-23 02:33:38', '2020-08-23 02:33:38'),
(22, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 17, '2020-08-23 02:46:09', '2020-08-23 02:46:09'),
(23, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 18, '2020-08-23 02:48:23', '2020-08-23 02:48:23'),
(24, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 19, '2020-08-23 03:08:31', '2020-08-23 03:08:31'),
(25, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 20, '2020-08-23 03:29:59', '2020-08-23 03:29:59'),
(26, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 21, '2020-08-23 05:54:38', '2020-08-23 05:54:38'),
(27, '/assets/images/profile_picture_vuduydu1997@gmail.com.png', 'James Blunt', 'facebook', 22, '2020-08-23 06:16:32', '2020-08-23 06:16:32'),
(28, '/assets/images/profile_picture_vuduyphuc98@gmail.com.png', 'Vũ Duy Phúc', 'facebook', 23, '2020-08-25 06:12:45', '2020-08-25 06:12:45'),
(29, '/assets/images/profile_picture_hoangluckyat@gmail.com.png', 'Hoàng', 'facebook', 24, '2020-09-06 02:42:14', '2020-09-06 02:42:14');

-- --------------------------------------------------------

--
-- Table structure for table `current_question_indices`
--

CREATE TABLE `current_question_indices` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `pack_id` bigint(20) NOT NULL,
  `playing_data_id` bigint(20) NOT NULL,
  `index` int(11) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `current_question_indices`
--

INSERT INTO `current_question_indices` (`id`, `pack_id`, `playing_data_id`, `index`, `created_at`, `updated_at`) VALUES
(31, 31, 21, 4, '2020-08-23 06:17:02', '2020-08-25 00:08:16'),
(32, 32, 21, 1, '2020-08-23 09:06:31', '2020-08-23 09:06:31'),
(33, 31, 22, 0, '2020-08-25 06:32:23', '2020-08-25 06:32:23'),
(34, 32, 22, 1, '2020-09-06 02:34:26', '2020-09-06 02:34:26'),
(35, 32, 23, 1, '2020-09-06 02:42:27', '2020-09-06 02:42:27'),
(36, 31, 23, 1, '2020-09-06 02:42:56', '2020-09-06 02:42:56');

-- --------------------------------------------------------

--
-- Table structure for table `failed_jobs`
--

CREATE TABLE `failed_jobs` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `connection` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `queue` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `payload` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `exception` longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `failed_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `mcq_questions`
--

CREATE TABLE `mcq_questions` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `pack_id` bigint(20) NOT NULL,
  `question` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `choices` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `answer` int(11) NOT NULL,
  `time` int(11) NOT NULL,
  `score` int(11) NOT NULL DEFAULT 0,
  `images` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hints` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `mcq_questions`
--

INSERT INTO `mcq_questions` (`id`, `pack_id`, `question`, `choices`, `answer`, `time`, `score`, `images`, `hints`, `created_at`, `updated_at`) VALUES
(14, 32, 'Đây là cờ nước nào?', '[\"Turkey\",\"Russia\",\"India\",\"Qatar\"]', 0, 30, 10, '[\"\\/assets\\/images\\/game_data\\/question_image_15989192270.jpg\",\"\\/assets\\/images\\/game_data\\/question_image_15989192271.jpg\",\"\\/assets\\/images\\/game_data\\/question_image_15989204800.jpg\"]', '[\"Khu vực biển đen\"]', '2020-08-22 23:53:58', '2020-08-31 17:34:40');

-- --------------------------------------------------------

--
-- Table structure for table `migrations`
--

CREATE TABLE `migrations` (
  `id` int(10) UNSIGNED NOT NULL,
  `migration` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `batch` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `migrations`
--

INSERT INTO `migrations` (`id`, `migration`, `batch`) VALUES
(4, '2014_10_12_000000_create_users_table', 1),
(5, '2019_08_19_000000_create_failed_jobs_table', 1),
(6, '2020_07_23_141503_create_auth_vendors_table', 1),
(7, '2020_08_17_051548_create_typed_questions_table', 2),
(8, '2020_08_17_052953_create_mcq_questions_table', 2),
(9, '2020_08_17_053242_create_packs_table', 2),
(10, '2020_08_17_053424_create_seasons_table', 2),
(11, '2020_08_19_015148_create_playing_data_table', 3),
(12, '2020_08_19_030145_create_current_question_indices_table', 3),
(14, '2020_08_19_030342_create_question_playing_data_table', 4);

-- --------------------------------------------------------

--
-- Table structure for table `packs`
--

CREATE TABLE `packs` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `season_id` bigint(20) NOT NULL,
  `title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sub_text` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'HUONG DAN',
  `icon` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `question_type` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `packs`
--

INSERT INTO `packs` (`id`, `season_id`, `title`, `sub_text`, `icon`, `question_type`, `created_at`, `updated_at`) VALUES
(31, 12, 'THỬ THÁCH', 'Hướng Dẫn', '/assets/images/game_data/icon_1598918618.jpg', '0', '2020-08-22 23:51:26', '2020-08-31 17:03:38'),
(32, 12, 'KIẾN THỨC', 'Trắc Nghiệm', '/assets/images/game_data/icon_1598918675.jpg', '1', '2020-08-22 23:53:10', '2020-08-31 17:04:35');

-- --------------------------------------------------------

--
-- Table structure for table `playing_data`
--

CREATE TABLE `playing_data` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `user_id` bigint(20) NOT NULL,
  `total_score` int(11) NOT NULL,
  `uptodate_token` int(11) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `playing_data`
--

INSERT INTO `playing_data` (`id`, `user_id`, `total_score`, `uptodate_token`, `created_at`, `updated_at`) VALUES
(21, 22, 20, 1599048102, '2020-08-23 06:16:38', '2020-09-02 05:01:42'),
(22, 23, 0, 1599384866, '2020-08-25 06:12:47', '2020-09-06 02:34:26'),
(23, 24, 10, 1599385376, '2020-09-06 02:42:16', '2020-09-06 02:42:56');

-- --------------------------------------------------------

--
-- Table structure for table `question_playing_data`
--

CREATE TABLE `question_playing_data` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `playing_data_id` bigint(20) NOT NULL,
  `season_id` bigint(20) NOT NULL,
  `pack_id` bigint(20) NOT NULL,
  `question_id` bigint(20) NOT NULL,
  `status` char(255) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'l',
  `started` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ended` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `used_hint_count` int(11) NOT NULL DEFAULT 0,
  `score` int(11) NOT NULL DEFAULT 0,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `question_playing_data`
--

INSERT INTO `question_playing_data` (`id`, `playing_data_id`, `season_id`, `pack_id`, `question_id`, `status`, `started`, `ended`, `used_hint_count`, `score`, `created_at`, `updated_at`) VALUES
(105, 21, 12, 31, 41, '99', '23/08/2020 13:16:41', '23/08/2020 13:20:28', 0, 10, '2020-08-23 06:17:02', '2020-08-23 06:20:47'),
(106, 21, 12, 31, 42, '99', '23/08/2020 13:20:30', '23/08/2020 13:57:57', 1, 10, '2020-08-23 06:20:47', '2020-08-23 06:59:31'),
(107, 21, 12, 31, 43, '99', '23/08/2020 13:58:08', '25/08/2020 09:07:41', 0, 10, '2020-08-23 06:59:31', '2020-08-25 00:08:16'),
(108, 21, 12, 32, 14, '119', '23/08/2020 16:06:21', '23/08/2020 16:06:28', 0, 10, '2020-08-23 09:06:32', '2020-08-23 09:06:32'),
(109, 21, 12, 31, 44, '99', '25/08/2020 09:07:43', '25/08/2020 09:08:07', 1, 0, '2020-08-25 00:08:17', '2020-08-25 00:08:17'),
(110, 22, 12, 31, 41, '97', '25/08/2020 13:32:10', NULL, 0, 0, '2020-08-25 06:32:23', '2020-08-25 06:32:23'),
(111, 22, 12, 31, 41, '97', '25/08/2020 13:32:10', NULL, 0, 0, '2020-08-25 06:32:24', '2020-08-25 06:32:24'),
(112, 22, 12, 32, 14, '119', '06/09/2020 09:34:04', '06/09/2020 09:34:21', 0, 10, '2020-09-06 02:34:26', '2020-09-06 02:34:26'),
(113, 23, 12, 32, 14, '99', '06/09/2020 09:42:16', '06/09/2020 09:42:17', 0, 10, '2020-09-06 02:42:27', '2020-09-06 02:42:27'),
(114, 23, 12, 31, 41, '99', '06/09/2020 09:42:31', '06/09/2020 09:42:47', 1, 0, '2020-09-06 02:42:56', '2020-09-06 02:42:56'),
(115, 23, 12, 31, 42, '97', '06/09/2020 09:42:48', NULL, 0, 0, '2020-09-06 02:42:56', '2020-09-06 02:42:56');

-- --------------------------------------------------------

--
-- Table structure for table `seasons`
--

CREATE TABLE `seasons` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `from` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `to` varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `seasons`
--

INSERT INTO `seasons` (`id`, `name`, `from`, `to`, `created_at`, `updated_at`) VALUES
(12, 'Mùa 1', '2020-08-22 00:00:00', '2020-08-26 00:00:00', '2020-08-22 23:50:49', '2020-09-02 05:01:35');

-- --------------------------------------------------------

--
-- Table structure for table `typed_questions`
--

CREATE TABLE `typed_questions` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `pack_id` bigint(20) NOT NULL,
  `question` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `answer` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `score` int(11) NOT NULL DEFAULT 0,
  `images` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hints` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `typed_questions`
--

INSERT INTO `typed_questions` (`id`, `pack_id`, `question`, `answer`, `score`, `images`, `hints`, `created_at`, `updated_at`) VALUES
(41, 31, 'Đây là cờ nước nào?', 'Mông Cổ', 10, '[\"\\/assets\\/images\\/game_data\\/question_image_15989191530.jpg\"]', '[\"Phía bắc trung quốc\"]', '2020-08-22 23:52:30', '2020-08-31 17:12:33'),
(42, 31, 'Hệ mặt trời có bao nhiêu hành tinh?', '9', 10, '[\"\\/assets\\/images\\/game_data\\/question_image_15989191630.jpg\"]', '[\"9\"]', '2020-08-23 00:09:33', '2020-08-31 17:12:44'),
(43, 31, 'Năm nay là năm gì?', 'Canh Tý', 10, '[\"\\/assets\\/images\\/game_data\\/question_image_15989191910.jpg\",\"\\/assets\\/images\\/game_data\\/question_image_15989191911.jpg\"]', '[\"Chuột\"]', '2020-08-23 02:05:37', '2020-08-31 17:13:11'),
(44, 31, '2021 là năm gì?', 'Tân Sửu', 10, '[\"\\/assets\\/images\\/game_data\\/question_image_15989192050.jpg\",\"\\/assets\\/images\\/game_data\\/question_image_15989192051.jpg\",\"\\/assets\\/images\\/game_data\\/question_image_15989192052.jpg\"]', '[\"Năm con trâu\"]', '2020-08-23 02:06:27', '2020-08-31 17:13:26');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `verification_code` int(11) NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `last_active_vendor_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `uptodate_token` int(11) NOT NULL DEFAULT 0,
  `remember_token` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `email`, `verification_code`, `password`, `last_active_vendor_name`, `uptodate_token`, `remember_token`, `created_at`, `updated_at`) VALUES
(22, 'vuduydu1997@gmail.com', 0, 'vuduydu', 'facebook', 1599144622, NULL, '2020-08-23 06:16:32', '2020-09-03 07:50:22'),
(23, 'vuduyphuc98@gmail.com', 0, 'vuduydu', 'facebook', 1599385401, NULL, '2020-08-25 06:12:45', '2020-09-06 02:43:21'),
(24, 'hoangluckyat@gmail.com', 0, 'vuduydu', 'facebook', 1599385331, NULL, '2020-09-06 02:42:11', '2020-09-06 02:42:11');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `auth_vendors`
--
ALTER TABLE `auth_vendors`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `current_question_indices`
--
ALTER TABLE `current_question_indices`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `failed_jobs`
--
ALTER TABLE `failed_jobs`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `mcq_questions`
--
ALTER TABLE `mcq_questions`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `migrations`
--
ALTER TABLE `migrations`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `packs`
--
ALTER TABLE `packs`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `playing_data`
--
ALTER TABLE `playing_data`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `question_playing_data`
--
ALTER TABLE `question_playing_data`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `seasons`
--
ALTER TABLE `seasons`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `typed_questions`
--
ALTER TABLE `typed_questions`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `users_email_unique` (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `auth_vendors`
--
ALTER TABLE `auth_vendors`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT for table `current_question_indices`
--
ALTER TABLE `current_question_indices`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT for table `failed_jobs`
--
ALTER TABLE `failed_jobs`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `mcq_questions`
--
ALTER TABLE `mcq_questions`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `migrations`
--
ALTER TABLE `migrations`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `packs`
--
ALTER TABLE `packs`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT for table `playing_data`
--
ALTER TABLE `playing_data`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT for table `question_playing_data`
--
ALTER TABLE `question_playing_data`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=116;

--
-- AUTO_INCREMENT for table `seasons`
--
ALTER TABLE `seasons`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `typed_questions`
--
ALTER TABLE `typed_questions`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=45;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
