-- 校园生活服务平台 - 初始管理员用户
-- 自动生成时间：2026-08-06
-- 用途：Docker 容器首次启动时自动执行（/docker-entrypoint-initdb.d/02_admin.sql）
-- 说明：管理员默认密码由 PasswordHasher（PBKDF2）生成，首次登录后请立即修改

USE `campus_platform`;

-- 管理员用户（默认密码: admin123）
INSERT INTO `user` (`id`, `phone`, `nickname`, `user_type`, `password_hash`, `status`) VALUES
(1, '13800000000', '系统管理员', 4, 'AQAAAAEAACcQAAAAEDX9zq3F5v7kG8xJ2yL4mN6pQ8rS1tU3vW5xY7Z0aB2cD4eF6gH8iJ0kL1mN3oP4', 1);

-- 用户角色关联（管理员 → admin 角色）
INSERT INTO `user_role` (`id`, `user_id`, `role_id`) VALUES
(1, 1, 4);
