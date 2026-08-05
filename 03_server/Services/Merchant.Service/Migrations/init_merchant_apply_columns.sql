-- 商家入驻字段扩展
-- 执行前请确认数据库名和表名与当前环境一致
-- 使用：mysql -u root -p campus_platform < init_merchant_apply_columns.sql

ALTER TABLE `merchant`
    ADD COLUMN `contact_name` VARCHAR(50) NULL COMMENT '联系人姓名' AFTER `phone`,
    ADD COLUMN `enterprise_name` VARCHAR(100) NULL COMMENT '企业名称' AFTER `contact_name`,
    ADD COLUMN `credit_code` VARCHAR(50) NULL COMMENT '统一社会信用代码' AFTER `enterprise_name`,
    ADD COLUMN `legal_person` VARCHAR(50) NULL COMMENT '法人姓名' AFTER `credit_code`,
    ADD COLUMN `business_category` VARCHAR(100) NULL COMMENT '经营类目' AFTER `legal_person`,
    ADD COLUMN `business_scope` VARCHAR(1000) NULL COMMENT '经营范围描述' AFTER `business_category`,
    ADD COLUMN `license_image` VARCHAR(500) NULL COMMENT '营业执照图片URL' AFTER `business_scope`,
    ADD COLUMN `id_card_front` VARCHAR(500) NULL COMMENT '身份证正面URL' AFTER `license_image`,
    ADD COLUMN `id_card_back` VARCHAR(500) NULL COMMENT '身份证反面URL' AFTER `id_card_front`,
    ADD COLUMN `sms_code` VARCHAR(10) NULL COMMENT '短信验证码（预留）' AFTER `id_card_back`,
    ADD COLUMN `agreed_terms` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否同意入驻协议' AFTER `sms_code`,
    ADD COLUMN `submit_step` INT NOT NULL DEFAULT 0 COMMENT '已提交步骤' AFTER `agreed_terms`,
    ADD COLUMN `audit_remark` VARCHAR(500) NULL COMMENT '审核备注' AFTER `submit_step`;
