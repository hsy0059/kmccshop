# 校园生活服务平台 (kmccshop)

面向大学校园场景的一站式生活服务平台，涵盖外卖订餐、校园跑腿、社交论坛、二手交易、失物招领、商家入驻审核、骑手配送、钱包优惠券等完整业务闭环。采用前后端分离 + 微服务架构，支持 H5 / 微信小程序 / Android 多端部署，并集成 three.js 3D 可视化组件与完整的单元 / E2E 测试体系。

## 技术栈

| 层级 | 技术 | 版本 | 说明 |
|------|------|------|------|
| 后端框架 | .NET | 10.0 | 微服务架构，8 个服务 + Ocelot 网关 |
| 后端 ORM | EF Core | - | MySQL 持久化，Code First Migration |
| API 网关 | Ocelot | - | 统一路由、聚合下游微服务 |
| 缓存 | Redis | 7.x | 会话与业务缓存 |
| 消息队列 | RabbitMQ | 3.x | 异步解耦 |
| 数据库 | MySQL | 8.0 | 主数据库 |
| 用户端 | UniApp + Vue 3 | - | H5 + 微信小程序（mp-weixin）|
| 管理端 / 商家端 | Vue 3 + Element Plus | - | Web 后台管理系统 |
| 3D 可视化 | three.js | 0.185 | 粒子背景 / 3D 柱状图 / 球形菜单 |
| 单元测试 | Vitest + jsdom | - | 29 个测试，three.js mock |
| E2E 测试 | Playwright | - | 16 个测试（Edge 浏览器）|
| 容器化 | Docker + docker-compose | 24+ | 基础设施与服务编排 |

## 项目结构

```
kmccXM/
├── 01_uniapp/uniapp    # 用户端 UniApp 小程序（H5 + mp-weixin）
├── 02_admin            # 管理端 Vue 3 + Element Plus Web
├── 02_merchant         # 商家端 Vue 3 + Element Plus Web
├── 03_server          # 后端 .NET 10 微服务（8 服务 + Ocelot 网关）
│   ├── ApiGateway      # Ocelot 网关（端口 53517）
│   ├── Common          # Campus.Common / Campus.Infrastructure 公共库
│   └── Services        # 8 个业务微服务
├── 04_sql              # 数据库初始化脚本
└── 05_doc              # 项目文档
```

## 快速开始

### 1. 启动基础设施

```bash
cd e:\kmccXM\03_server
docker-compose up -d
# 启动 MySQL(:3306) + Redis(:6379) + RabbitMQ(:5672/:15672)
```

### 2. 初始化数据库

```bash
mysql -u root -proot123 < e:\kmccXM\04_sql\sql\init_database.sql
mysql -u root -proot123 < e:\kmccXM\04_sql\sql\init_admin_user.sql
```

管理员初始账号：`admin` / 密码：`admin123`

### 3. 启动后端微服务

```bash
cd e:\kmccXM\03_server
.\start-all-services.ps1   # 一键启动全部微服务 + 网关
```

### 4. 启动前端

```bash
# 管理端（http://localhost:3000）
cd e:\kmccXM\02_admin && npm install && npm run dev

# 商家端（http://localhost:3001）
cd e:\kmccXM\02_merchant && npm install && npm run dev

# 用户端 H5（http://localhost:8080）
cd e:\kmccXM\01_uniapp\uniapp && npm install && npm run dev
```

## 测试

```bash
# 单元测试（Vitest + jsdom）
cd e:\kmccXM\02_admin && npm test            # ParticleBackground(9) + Bar3DChart(11) = 20 个
cd e:\kmccXM\01_uniapp\uniapp && npm test     # SphereMenu = 9 个
# 合计 29 个单元测试，全部通过

# E2E 测试（Playwright + Edge 浏览器，在 02_admin 目录）
cd e:\kmccXM\02_admin && npm run e2e          # 3d-components.spec.ts = 16 个
```

## 后端微服务端口

| 服务 | 端口 | 职责 |
|------|------|------|
| ApiGateway（网关）| 53517 | Ocelot 统一入口 |
| Campus.Service | 53211 | 校区 / 配送区域 |
| Social.Service | 53215 | 帖子 / 二手 / 失物 / 广告 |
| Order.Service | 53216 | 订单 / 跑腿 |
| Delivery.Service | 53221 | 骑手 |
| User.Service | 53222 | 用户 / 认证 / 地址 / 反馈 / 文件 |
| Merchant.Service | 53523 | 商家 / 商品 / 入驻审核 |
| Wallet.Service | 53224 | 钱包 / 流水 / 提现 |
| Coupon.Service | 53225 | 优惠券 |

## 文档索引

| 文档 | 路径 | 内容 |
|------|------|------|
| 开发文档 | [05_doc/开发文档.md](./05_doc/开发文档.md) | 开发环境、快速开始、前后端开发指南、数据库、API、3D 组件、测试体系、部署、AI 方案 |
| 环境配置与编译指南 | [05_doc/环境配置与编译指南.md](./05_doc/环境配置与编译指南.md) | 环境配置与编译说明 |
| 详细开发表 | [05_doc/详细开发表.md](./05_doc/详细开发表.md) | 详细开发任务表 |
| 项目立项报告 | [05_doc/项目立项报告.md](./05_doc/项目立项报告.md) | 项目立项说明 |
| 项目系统说明书 | [05_doc/项目系统说明书.md](./05_doc/项目系统说明书.md) | 系统设计说明 |
| 项目需求规格说明书 | [05_doc/项目需求规格说明书.md](./05_doc/项目需求规格说明书.md) | 需求规格说明 |

> 各文档详细内容见 `05_doc/` 目录。
