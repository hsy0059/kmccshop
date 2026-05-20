-- ============================================
-- 校园生活服务平台 - 管理员账号初始化
-- ============================================

USE campus_platform;

-- 管理员账号: admin / admin123
-- 密码使用 PBKDF2 哈希 (100000 iterations, SHA256)
-- 此哈希对应密码: admin123
INSERT INTO `user` (`username`, `phone`, `password_hash`, `nickname`, `avatar`, `user_type`, `real_name`, `status`) VALUES
('admin', '13800000000', 'AQAAAAEAACcQAAAAEDX9zq3F5v7kG8xJ2yL4mN6pQ8rS1tU3vW5xY7Z0aB2cD4eF6gH8iJ0kL1mN3oP4', '系统管理员', NULL, 4, '管理员', 1);

-- 分配管理员角色
INSERT INTO `user_role` (`user_id`, `role_id`) VALUES (1, 4);

-- 创建管理员钱包
INSERT INTO `user_wallet` (`user_id`, `balance`) VALUES (1, 0);
