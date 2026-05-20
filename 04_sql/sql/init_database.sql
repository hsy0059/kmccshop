-- ============================================
-- 校园生活服务平台 - 数据库初始化脚本
-- 版本: 1.0.0 | 更新时间: 2026-05-08
-- ============================================

CREATE DATABASE IF NOT EXISTS campus_platform DEFAULT CHARSET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE campus_platform;

-- ============================================
-- 基础服务
-- ============================================

DROP TABLE IF EXISTS `school`;
CREATE TABLE `school` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL COMMENT '学校名称',
  `short_name` VARCHAR(50) DEFAULT NULL COMMENT '简称',
  `province` VARCHAR(50) DEFAULT NULL,
  `city` VARCHAR(50) DEFAULT NULL,
  `district` VARCHAR(50) DEFAULT NULL,
  `address` VARCHAR(255) DEFAULT NULL,
  `logo` VARCHAR(500) DEFAULT NULL,
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-禁用 1-启用',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='学校信息';

DROP TABLE IF EXISTS `campus`;
CREATE TABLE `campus` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `school_id` BIGINT NOT NULL COMMENT '所属学校ID',
  `name` VARCHAR(100) NOT NULL COMMENT '校区名称',
  `address` VARCHAR(255) DEFAULT NULL,
  `longitude` DECIMAL(10,6) DEFAULT NULL COMMENT '经度',
  `latitude` DECIMAL(10,6) DEFAULT NULL COMMENT '纬度',
  `delivery_radius` INT DEFAULT 3000 COMMENT '配送半径(米)',
  `status` TINYINT NOT NULL DEFAULT 1,
  `sort_order` INT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_school_id` (`school_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='校区信息';

DROP TABLE IF EXISTS `delivery_zone`;
CREATE TABLE `delivery_zone` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `campus_id` BIGINT NOT NULL,
  `name` VARCHAR(100) NOT NULL COMMENT '区域名称',
  `delivery_fee` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '配送费',
  `min_order_amount` DECIMAL(10,2) DEFAULT 0 COMMENT '起送金额',
  `estimated_time` INT DEFAULT 30 COMMENT '预计送达时间(分钟)',
  `status` TINYINT NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_campus_id` (`campus_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='配送区域';

-- ============================================
-- 用户服务
-- ============================================

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) DEFAULT NULL COMMENT '用户名',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `password_hash` VARCHAR(255) DEFAULT NULL COMMENT '密码哈希',
  `nickname` VARCHAR(50) DEFAULT NULL COMMENT '昵称',
  `avatar` VARCHAR(500) DEFAULT NULL COMMENT '头像',
  `gender` TINYINT DEFAULT 0 COMMENT '0-未知 1-男 2-女',
  `email` VARCHAR(100) DEFAULT NULL,
  `wechat_openid` VARCHAR(100) DEFAULT NULL COMMENT '微信OpenID',
  `wechat_unionid` VARCHAR(100) DEFAULT NULL,
  `user_type` TINYINT NOT NULL DEFAULT 1 COMMENT '1-学生 2-商家 3-骑手 4-管理员',
  `student_id` VARCHAR(50) DEFAULT NULL COMMENT '学号',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `school_id` BIGINT DEFAULT NULL,
  `campus_id` BIGINT DEFAULT NULL,
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-禁用 1-正常',
  `last_login_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_phone` (`phone`),
  UNIQUE KEY `uk_wechat_openid` (`wechat_openid`),
  KEY `idx_user_type` (`user_type`),
  KEY `idx_school_id` (`school_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';

DROP TABLE IF EXISTS `role`;
CREATE TABLE `role` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL COMMENT '角色名称',
  `code` VARCHAR(50) NOT NULL COMMENT '角色编码',
  `description` VARCHAR(255) DEFAULT NULL,
  `status` TINYINT NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';

DROP TABLE IF EXISTS `user_role`;
CREATE TABLE `user_role` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `role_id` BIGINT NOT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_role` (`user_id`, `role_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_role_id` (`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联';

DROP TABLE IF EXISTS `user_address`;
CREATE TABLE `user_address` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `contact_name` VARCHAR(50) NOT NULL COMMENT '联系人',
  `contact_phone` VARCHAR(20) NOT NULL COMMENT '联系电话',
  `province` VARCHAR(50) DEFAULT NULL,
  `city` VARCHAR(50) DEFAULT NULL,
  `district` VARCHAR(50) DEFAULT NULL,
  `detail` VARCHAR(255) NOT NULL COMMENT '详细地址',
  `longitude` DECIMAL(10,6) DEFAULT NULL,
  `latitude` DECIMAL(10,6) DEFAULT NULL,
  `is_default` TINYINT NOT NULL DEFAULT 0 COMMENT '是否默认地址',
  `tag` VARCHAR(50) DEFAULT NULL COMMENT '标签(家/公司/学校)',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户收货地址';

-- ============================================
-- 骑手服务
-- ============================================

DROP TABLE IF EXISTS `rider`;
CREATE TABLE `rider` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `real_name` VARCHAR(50) NOT NULL,
  `phone` VARCHAR(20) NOT NULL,
  `id_card` VARCHAR(20) DEFAULT NULL COMMENT '身份证号',
  `balance` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '骑手余额',
  `rating` DECIMAL(2,1) DEFAULT 5.0 COMMENT '评分',
  `order_count` INT DEFAULT 0 COMMENT '完成订单数',
  `status` TINYINT NOT NULL DEFAULT 0 COMMENT '0-休息 1-接单中 2-配送中 3-禁用',
  `audit_status` TINYINT NOT NULL DEFAULT 0 COMMENT '0-待审核 1-审核通过 2-审核拒绝',
  `vehicle_type` VARCHAR(50) DEFAULT NULL COMMENT '交通工具',
  `vehicle_number` VARCHAR(50) DEFAULT NULL,
  `campus_id` BIGINT DEFAULT NULL COMMENT '服务校区',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_campus_id` (`campus_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='骑手信息';

-- ============================================
-- 商家服务
-- ============================================

DROP TABLE IF EXISTS `merchant`;
CREATE TABLE `merchant` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL COMMENT '关联用户ID',
  `name` VARCHAR(100) NOT NULL COMMENT '商家名称',
  `logo` VARCHAR(500) DEFAULT NULL,
  `cover_image` VARCHAR(500) DEFAULT NULL,
  `phone` VARCHAR(20) DEFAULT NULL,
  `description` VARCHAR(500) DEFAULT NULL,
  `address` VARCHAR(255) DEFAULT NULL,
  `business_hours` VARCHAR(100) DEFAULT NULL COMMENT '营业时间',
  `min_delivery_amount` DECIMAL(10,2) DEFAULT 0 COMMENT '起送价',
  `delivery_fee` DECIMAL(10,2) DEFAULT 0 COMMENT '配送费',
  `rating` DECIMAL(2,1) DEFAULT 5.0,
  `monthly_sales` INT DEFAULT 0 COMMENT '月销量',
  `status` TINYINT NOT NULL DEFAULT 0 COMMENT '0-待审核 1-营业中 2-休息中 3-已禁用',
  `campus_id` BIGINT DEFAULT NULL,
  `longitude` DECIMAL(10,6) DEFAULT NULL,
  `latitude` DECIMAL(10,6) DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_campus_id` (`campus_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商家信息';

DROP TABLE IF EXISTS `category`;
CREATE TABLE `category` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `merchant_id` BIGINT NOT NULL,
  `name` VARCHAR(50) NOT NULL COMMENT '分类名称',
  `icon` VARCHAR(500) DEFAULT NULL,
  `sort_order` INT DEFAULT 0,
  `status` TINYINT NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_merchant_id` (`merchant_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品分类';

DROP TABLE IF EXISTS `product`;
CREATE TABLE `product` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `merchant_id` BIGINT NOT NULL,
  `category_id` BIGINT DEFAULT NULL,
  `name` VARCHAR(100) NOT NULL COMMENT '商品名称',
  `description` VARCHAR(500) DEFAULT NULL,
  `image` VARCHAR(500) DEFAULT NULL COMMENT '主图',
  `images` TEXT DEFAULT NULL COMMENT '多图(JSON)',
  `price` DECIMAL(10,2) NOT NULL COMMENT '原价',
  `discount_price` DECIMAL(10,2) DEFAULT NULL COMMENT '折扣价',
  `stock` INT NOT NULL DEFAULT 0 COMMENT '库存',
  `sales` INT DEFAULT 0 COMMENT '销量',
  `unit` VARCHAR(20) DEFAULT '份' COMMENT '单位',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-下架 1-上架',
  `is_recommend` TINYINT DEFAULT 0 COMMENT '是否推荐',
  `sort_order` INT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_merchant_id` (`merchant_id`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品表';

DROP TABLE IF EXISTS `product_spec`;
CREATE TABLE `product_spec` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `product_id` BIGINT NOT NULL,
  `name` VARCHAR(100) NOT NULL COMMENT '规格名称',
  `price` DECIMAL(10,2) NOT NULL COMMENT '规格价格',
  `stock` INT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_product_id` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品规格';

-- ============================================
-- 订单服务
-- ============================================

DROP TABLE IF EXISTS `delivery_order`;
CREATE TABLE `delivery_order` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `order_no` VARCHAR(32) NOT NULL COMMENT '订单编号',
  `user_id` BIGINT NOT NULL COMMENT '下单用户',
  `merchant_id` BIGINT NOT NULL COMMENT '商家ID',
  `rider_id` BIGINT DEFAULT NULL COMMENT '骑手ID',
  `address_id` BIGINT DEFAULT NULL COMMENT '收货地址ID',
  `total_amount` DECIMAL(10,2) NOT NULL COMMENT '商品总金额',
  `delivery_fee` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '配送费',
  `discount_amount` DECIMAL(10,2) DEFAULT 0 COMMENT '优惠金额',
  `actual_amount` DECIMAL(10,2) NOT NULL COMMENT '实付金额',
  `payment_method` TINYINT DEFAULT NULL COMMENT '1-余额 2-微信支付',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '1-待支付 2-待接单 3-已接单 4-配送中 5-已送达 6-已取消 7-已完成',
  `remark` VARCHAR(255) DEFAULT NULL COMMENT '备注',
  `delivery_time` DATETIME DEFAULT NULL COMMENT '配送时间',
  `cancel_reason` VARCHAR(255) DEFAULT NULL COMMENT '取消原因',
  `refund_status` TINYINT DEFAULT 0 COMMENT '0-无退款 1-退款中 2-已退款 3-退款拒绝',
  `refund_amount` DECIMAL(10,2) DEFAULT NULL,
  `paid_at` DATETIME DEFAULT NULL COMMENT '支付时间',
  `completed_at` DATETIME DEFAULT NULL COMMENT '完成时间',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_order_no` (`order_no`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_merchant_id` (`merchant_id`),
  KEY `idx_rider_id` (`rider_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='外卖订单';

DROP TABLE IF EXISTS `order_item`;
CREATE TABLE `order_item` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `order_id` BIGINT NOT NULL,
  `product_id` BIGINT NOT NULL,
  `product_name` VARCHAR(100) NOT NULL COMMENT '商品名称(快照)',
  `product_image` VARCHAR(500) DEFAULT NULL,
  `spec_name` VARCHAR(100) DEFAULT NULL COMMENT '规格名称',
  `price` DECIMAL(10,2) NOT NULL COMMENT '单价',
  `quantity` INT NOT NULL DEFAULT 1 COMMENT '数量',
  `total_price` DECIMAL(10,2) NOT NULL COMMENT '小计',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_product_id` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单商品明细';

DROP TABLE IF EXISTS `errand_order`;
CREATE TABLE `errand_order` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `order_no` VARCHAR(32) NOT NULL COMMENT '订单编号',
  `user_id` BIGINT NOT NULL COMMENT '发布用户',
  `rider_id` BIGINT DEFAULT NULL COMMENT '接单骑手',
  `title` VARCHAR(100) NOT NULL COMMENT '跑腿标题',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '需求描述',
  `pickup_address` VARCHAR(255) NOT NULL COMMENT '取件地址',
  `delivery_address` VARCHAR(255) NOT NULL COMMENT '送达地址',
  `tip_amount` DECIMAL(10,2) NOT NULL COMMENT '小费金额',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '1-待接单 2-已接单 3-配送中 4-已完成 5-已取消',
  `contact_name` VARCHAR(50) DEFAULT NULL,
  `contact_phone` VARCHAR(20) DEFAULT NULL,
  `remark` VARCHAR(255) DEFAULT NULL,
  `picked_at` DATETIME DEFAULT NULL COMMENT '取件时间',
  `completed_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_order_no` (`order_no`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_rider_id` (`rider_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='跑腿订单';

DROP TABLE IF EXISTS `order_comment`;
CREATE TABLE `order_comment` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `order_id` BIGINT NOT NULL,
  `user_id` BIGINT NOT NULL COMMENT '评论用户',
  `target_id` BIGINT NOT NULL COMMENT '被评价对象ID(商家/骑手)',
  `target_type` TINYINT NOT NULL COMMENT '1-商家 2-骑手',
  `rating` TINYINT NOT NULL COMMENT '评分(1-5)',
  `content` VARCHAR(500) DEFAULT NULL COMMENT '评论内容',
  `images` TEXT DEFAULT NULL COMMENT '图片(JSON)',
  `reply_content` VARCHAR(500) DEFAULT NULL COMMENT '回复内容',
  `replied_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_target_id` (`target_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单评价';

-- ============================================
-- 社区服务
-- ============================================

DROP TABLE IF EXISTS `post_category`;
CREATE TABLE `post_category` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL COMMENT '分类名称',
  `icon` VARCHAR(500) DEFAULT NULL,
  `sort_order` INT DEFAULT 0,
  `status` TINYINT NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帖子分类';

DROP TABLE IF EXISTS `post`;
CREATE TABLE `post` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `category_id` BIGINT DEFAULT NULL,
  `title` VARCHAR(100) NOT NULL COMMENT '标题',
  `content` TEXT NOT NULL COMMENT '内容',
  `images` TEXT DEFAULT NULL COMMENT '图片(JSON)',
  `view_count` INT DEFAULT 0 COMMENT '浏览数',
  `like_count` INT DEFAULT 0 COMMENT '点赞数',
  `comment_count` INT DEFAULT 0 COMMENT '评论数',
  `favorite_count` INT DEFAULT 0 COMMENT '收藏数',
  `is_top` TINYINT DEFAULT 0 COMMENT '是否置顶',
  `is_essence` TINYINT DEFAULT 0 COMMENT '是否精华',
  `is_locked` TINYINT DEFAULT 0 COMMENT '是否锁定',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-删除 1-正常 2-审核中',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_top` (`is_top`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帖子';

DROP TABLE IF EXISTS `post_like`;
CREATE TABLE `post_like` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `post_id` BIGINT NOT NULL,
  `user_id` BIGINT NOT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_post_user` (`post_id`, `user_id`),
  KEY `idx_post_id` (`post_id`),
  KEY `idx_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帖子点赞';

DROP TABLE IF EXISTS `post_comment`;
CREATE TABLE `post_comment` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `post_id` BIGINT NOT NULL,
  `user_id` BIGINT NOT NULL COMMENT '评论用户',
  `parent_id` BIGINT DEFAULT NULL COMMENT '父评论ID(多级回复)',
  `reply_to_user_id` BIGINT DEFAULT NULL COMMENT '回复目标用户ID',
  `content` VARCHAR(500) NOT NULL COMMENT '评论内容',
  `like_count` INT DEFAULT 0,
  `status` TINYINT NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_post_id` (`post_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='帖子评论';

DROP TABLE IF EXISTS `second_goods`;
CREATE TABLE `second_goods` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL COMMENT '发布用户',
  `title` VARCHAR(100) NOT NULL COMMENT '标题',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `images` TEXT DEFAULT NULL COMMENT '图片(JSON)',
  `price` DECIMAL(10,2) NOT NULL COMMENT '价格',
  `original_price` DECIMAL(10,2) DEFAULT NULL COMMENT '原价',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '分类',
  `condition_desc` VARCHAR(50) DEFAULT NULL COMMENT '成色描述',
  `view_count` INT DEFAULT 0,
  `favorite_count` INT DEFAULT 0,
  `contact_info` VARCHAR(100) DEFAULT NULL COMMENT '联系方式',
  `campus_id` BIGINT DEFAULT NULL COMMENT '所在校区',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-删除 1-在售 2-已售',
  `is_sold` TINYINT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_campus_id` (`campus_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='二手商品';

DROP TABLE IF EXISTS `lost_found`;
CREATE TABLE `lost_found` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `type` TINYINT NOT NULL COMMENT '1-寻物 2-招领',
  `title` VARCHAR(100) NOT NULL COMMENT '标题',
  `description` VARCHAR(500) NOT NULL COMMENT '描述',
  `images` TEXT DEFAULT NULL COMMENT '图片(JSON)',
  `location` VARCHAR(100) DEFAULT NULL COMMENT '遗失/捡到地点',
  `contact_info` VARCHAR(100) DEFAULT NULL COMMENT '联系方式',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '物品分类',
  `campus_id` BIGINT DEFAULT NULL,
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-删除 1-寻找中 2-已归还 3-已过期',
  `view_count` INT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_type` (`type`),
  KEY `idx_status` (`status`),
  KEY `idx_campus_id` (`campus_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='失物招领';

-- ============================================
-- 钱包服务
-- ============================================

DROP TABLE IF EXISTS `user_wallet`;
CREATE TABLE `user_wallet` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `balance` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '可用余额',
  `frozen_balance` DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '冻结余额',
  `total_income` DECIMAL(10,2) DEFAULT 0 COMMENT '累计收入',
  `total_expense` DECIMAL(10,2) DEFAULT 0 COMMENT '累计支出',
  `pay_password` VARCHAR(255) DEFAULT NULL COMMENT '支付密码',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户钱包';

DROP TABLE IF EXISTS `wallet_log`;
CREATE TABLE `wallet_log` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `type` TINYINT NOT NULL COMMENT '1-充值 2-消费 3-退款 4-收入 5-提现 6-冻结 7-解冻',
  `amount` DECIMAL(10,2) NOT NULL COMMENT '金额(正数为收入，负数为支出)',
  `balance_before` DECIMAL(10,2) DEFAULT NULL COMMENT '变动前余额',
  `balance_after` DECIMAL(10,2) DEFAULT NULL COMMENT '变动后余额',
  `related_id` BIGINT DEFAULT NULL COMMENT '关联订单/提现ID',
  `related_type` VARCHAR(50) DEFAULT NULL COMMENT '关联类型',
  `description` VARCHAR(255) DEFAULT NULL COMMENT '描述',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_type` (`type`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='钱包流水';

DROP TABLE IF EXISTS `withdraw`;
CREATE TABLE `withdraw` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `amount` DECIMAL(10,2) NOT NULL COMMENT '提现金额',
  `fee` DECIMAL(10,2) DEFAULT 0 COMMENT '手续费',
  `actual_amount` DECIMAL(10,2) NOT NULL COMMENT '实际到账',
  `account_type` VARCHAR(50) DEFAULT NULL COMMENT '到账方式(微信/支付宝/银行卡)',
  `account_info` VARCHAR(255) DEFAULT NULL COMMENT '账号信息',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '1-待审核 2-审核通过 3-打款中 4-已完成 5-已拒绝',
  `reject_reason` VARCHAR(255) DEFAULT NULL COMMENT '拒绝原因',
  `auditor_id` BIGINT DEFAULT NULL COMMENT '审核人ID',
  `audited_at` DATETIME DEFAULT NULL,
  `remark` VARCHAR(255) DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='提现申请';

-- ============================================
-- 系统服务
-- ============================================

DROP TABLE IF EXISTS `advertisement`;
CREATE TABLE `advertisement` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(100) NOT NULL COMMENT '广告标题',
  `image` VARCHAR(500) NOT NULL COMMENT '广告图片',
  `link_url` VARCHAR(500) DEFAULT NULL COMMENT '链接地址',
  `position` VARCHAR(50) NOT NULL COMMENT '广告位(banner/home/sidebar)',
  `sort_order` INT DEFAULT 0,
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-禁用 1-启用',
  `click_count` INT DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_position` (`position`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='广告管理';

DROP TABLE IF EXISTS `feedback`;
CREATE TABLE `feedback` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `type` TINYINT NOT NULL COMMENT '1-问题反馈 2-功能建议 3-其他',
  `title` VARCHAR(100) NOT NULL,
  `content` VARCHAR(500) NOT NULL COMMENT '反馈内容',
  `images` TEXT DEFAULT NULL COMMENT '图片(JSON)',
  `contact_info` VARCHAR(100) DEFAULT NULL,
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '1-待处理 2-处理中 3-已回复 4-已关闭',
  `reply_content` VARCHAR(500) DEFAULT NULL COMMENT '回复内容',
  `replier_id` BIGINT DEFAULT NULL COMMENT '回复人ID',
  `replied_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户反馈';

-- ============================================
-- 优惠券服务
-- ============================================

DROP TABLE IF EXISTS `coupon`;
CREATE TABLE `coupon` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL COMMENT '优惠券名称',
  `description` VARCHAR(255) DEFAULT NULL,
  `type` TINYINT NOT NULL COMMENT '1-满减 2-折扣 3-无门槛',
  `discount_value` DECIMAL(10,2) NOT NULL COMMENT '优惠金额/折扣率',
  `min_amount` DECIMAL(10,2) DEFAULT 0 COMMENT '最低消费金额',
  `max_discount` DECIMAL(10,2) DEFAULT NULL COMMENT '最大优惠金额',
  `total_count` INT NOT NULL DEFAULT 0 COMMENT '发放总量',
  `received_count` INT DEFAULT 0 COMMENT '已领取量',
  `used_count` INT DEFAULT 0 COMMENT '已使用量',
  `per_user_limit` INT DEFAULT 1 COMMENT '每人限领',
  `merchant_id` BIGINT DEFAULT NULL COMMENT '适用商家(NULL=全平台)',
  `start_time` DATETIME NOT NULL COMMENT '生效时间',
  `end_time` DATETIME NOT NULL COMMENT '过期时间',
  `valid_days` INT DEFAULT NULL COMMENT '领取后有效天数',
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '0-禁用 1-启用 2-已过期',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_merchant_id` (`merchant_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='优惠券模板';

DROP TABLE IF EXISTS `user_coupon`;
CREATE TABLE `user_coupon` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `user_id` BIGINT NOT NULL,
  `coupon_id` BIGINT NOT NULL,
  `status` TINYINT NOT NULL DEFAULT 1 COMMENT '1-未使用 2-已使用 3-已过期',
  `order_id` BIGINT DEFAULT NULL COMMENT '使用订单ID',
  `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '领取时间',
  `used_at` DATETIME DEFAULT NULL COMMENT '使用时间',
  `expire_at` DATETIME DEFAULT NULL COMMENT '过期时间',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_coupon_id` (`coupon_id`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户持有的优惠券';

-- ============================================
-- 初始数据
-- ============================================

INSERT INTO `school` (`name`, `short_name`, `status`) VALUES ('示范大学', '示范大', 1);

INSERT INTO `campus` (`school_id`, `name`, `address`, `longitude`, `latitude`, `delivery_radius`) VALUES
(1, '主校区', '示范大学路1号', 116.397428, 39.908722, 3000),
(1, '东校区', '示范大学路2号', 116.407428, 39.918722, 3000);

INSERT INTO `role` (`name`, `code`, `description`) VALUES
('学生', 'student', '普通学生用户'),
('商家', 'merchant', '入驻商家'),
('骑手', 'rider', '配送骑手'),
('管理员', 'admin', '平台管理员');

INSERT INTO `post_category` (`name`, `sort_order`) VALUES
('校园生活', 1),
('学习交流', 2),
('二手交易', 3),
('失物招领', 4),
('求助咨询', 5),
('美食推荐', 6),
('休闲娱乐', 7),
('其他', 99);

INSERT INTO `delivery_zone` (`campus_id`, `name`, `delivery_fee`, `min_order_amount`, `estimated_time`) VALUES
(1, '教学楼A区', 2.00, 10.00, 25),
(1, '教学楼B区', 2.00, 10.00, 25),
(1, '宿舍楼1-5栋', 1.00, 8.00, 20),
(1, '宿舍楼6-10栋', 1.50, 8.00, 22),
(2, '东校区教学楼', 2.50, 12.00, 30),
(2, '东校区宿舍楼', 2.00, 10.00, 28);
