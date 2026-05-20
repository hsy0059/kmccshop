<template>
  <el-card>
    <template #header><div class="card-header"><span>数据脚本</span></div></template>
    <el-tabs><el-tab-pane label="数据库初始化">
      <el-alert title="初始化数据库表结构" type="info" :closable="false" style="margin-bottom:16px" />
      <el-input type="textarea" rows="16" model-value="-- 执行 04_sql/sql/init_database.sql 脚本" readonly />
    </el-tab-pane><el-tab-pane label="管理员初始化">
      <el-alert title="初始化管理员账号" type="info" :closable="false" style="margin-bottom:16px" />
      <el-input type="textarea" rows="8" model-value="-- 执行 04_sql/sql/init_admin_user.sql 脚本&#10;-- 管理员账号: admin / admin123" readonly />
    </el-tab-pane><el-tab-pane label="索引脚本">
      <el-input type="textarea" rows="8" model-value="CREATE INDEX idx_created ON delivery_order(created_at);&#10;CREATE INDEX idx_status_created ON delivery_order(status, created_at);" readonly />
    </el-tab-pane></el-tabs>
  </el-card>
</template>