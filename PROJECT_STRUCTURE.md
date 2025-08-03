# 📁 Wordle项目目录结构

## 🏗️ 项目架构

```
wordle/
├── 📁 src/                          # 源代码目录
│   ├── 📁 WordleGame.Core/          # 核心游戏逻辑
│   │   ├── WordleGame.cs            # 基础游戏类
│   │   ├── GameConfiguration.cs     # 配置管理
│   │   └── WordleGame.Core.csproj   # 项目文件
│   │
│   ├── 📁 WordleGame.Server/        # 服务器项目
│   │   ├── WordleServer.cs          # 服务器实现
│   │   ├── Program.cs               # 服务器入口
│   │   └── WordleGame.Server.csproj # 项目文件
│   │
│   ├── 📁 WordleGame.Client/        # 客户端项目
│   │   ├── WordleClient.cs          # 客户端实现
│   │   ├── Program.cs               # 客户端入口
│   │   └── WordleGame.Client.csproj # 项目文件
│   │
│   └── 📁 WordleGame.Cheating/      # 作弊模式项目
│       ├── CheatingWordleGame.cs    # 作弊游戏逻辑
│       ├── CheatingWordleServer.cs  # 作弊服务器
│       ├── CheatingWordleClient.cs  # 作弊客户端
│       ├── Program.cs               # 作弊模式入口
│       └── WordleGame.Cheating.csproj # 项目文件
│
├── 📁 tests/                        # 测试目录
│   └── 📁 WordleGame.Tests/         # 测试项目
│       ├── 📁 UnitTests/            # 单元测试
│       │   ├── GameConfigurationTests.cs
│       │   └── WordleGameTests.cs
│       ├── 📁 IntegrationTests/     # 集成测试
│       │   └── ServerClientIntegrationTests.cs
│       └── WordleGame.Tests.csproj  # 测试项目文件
│
├── 📁 scripts/                      # 脚本目录
│   ├── start-server.bat             # 启动服务器
│   ├── start-client.bat             # 启动客户端
│   ├── start-cheating-server.bat    # 启动作弊服务器
│   └── start-cheating-client.bat    # 启动作弊客户端
│
├── 📁 config/                       # 配置文件目录
│   └── words.json                   # 单词列表配置
│
├── 📁 docs/                         # 文档目录
│   ├── README.md                    # 项目说明
│   ├── BONUS_FEATURES.md           # 增强功能设计
│   └── PROJECT_STRUCTURE.md        # 项目结构说明
│
├── wordle.sln                       # 解决方案文件
├── .gitignore                       # Git忽略文件
└── README.md                        # 项目根目录说明
```

## 🎯 项目说明

### 📦 核心项目 (WordleGame.Core)
- **职责**：包含所有共享的游戏逻辑和配置
- **包含**：基础游戏类、配置管理、枚举定义
- **依赖**：System.Text.Json

### 🖥️ 服务器项目 (WordleGame.Server)
- **职责**：处理客户端连接和游戏会话
- **包含**：TCP服务器实现、会话管理
- **依赖**：WordleGame.Core

### 💻 客户端项目 (WordleGame.Client)
- **职责**：用户界面和网络通信
- **包含**：客户端实现、用户交互
- **依赖**：WordleGame.Core

### 🎲 作弊模式项目 (WordleGame.Cheating)
- **职责**：实现Absurdle风格的作弊功能
- **包含**：作弊游戏逻辑、动态候选词管理
- **依赖**：WordleGame.Core

### 🧪 测试项目 (WordleGame.Tests)
- **职责**：单元测试和集成测试
- **包含**：xUnit测试框架、测试用例
- **依赖**：所有其他项目

## 🚀 构建和运行

### 构建整个解决方案
```bash
dotnet build wordle.sln
```

### 运行特定项目
```bash
# 运行服务器
dotnet run --project src/WordleGame.Server/WordleGame.Server.csproj

# 运行客户端
dotnet run --project src/WordleGame.Client/WordleGame.Client.csproj

# 运行作弊模式
dotnet run --project src/WordleGame.Cheating/WordleGame.Cheating.csproj

# 运行测试
dotnet test tests/WordleGame.Tests/WordleGame.Tests.csproj
```

### 使用脚本运行
```bash
# Windows
scripts/start-server.bat
scripts/start-client.bat
scripts/start-cheating-server.bat
scripts/start-cheating-client.bat
```

## 🧪 测试

### 运行所有测试
```bash
dotnet test
```

### 运行特定测试
```bash
# 运行单元测试
dotnet test --filter "Category=Unit"

# 运行集成测试
dotnet test --filter "Category=Integration"
```

### 代码覆盖率
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📋 开发工作流

1. **修改核心逻辑** → 更新 `src/WordleGame.Core/`
2. **修改服务器** → 更新 `src/WordleGame.Server/`
3. **修改客户端** → 更新 `src/WordleGame.Client/`
4. **修改作弊模式** → 更新 `src/WordleGame.Cheating/`
5. **添加测试** → 更新 `tests/WordleGame.Tests/`
6. **更新配置** → 修改 `config/words.json`
7. **更新脚本** → 修改 `scripts/` 目录

## 🔧 配置

### 单词列表
- **位置**：`config/words.json`
- **格式**：JSON数组，包含5字母单词
- **默认**：100个常用5字母单词

### 网络设置
- **默认端口**：8888 (普通模式), 8889 (作弊模式)
- **默认地址**：localhost
- **协议**：TCP

## 📊 项目依赖关系

```
WordleGame.Tests
├── WordleGame.Core
├── WordleGame.Server
├── WordleGame.Client
└── WordleGame.Cheating

WordleGame.Server
└── WordleGame.Core

WordleGame.Client
└── WordleGame.Core

WordleGame.Cheating
└── WordleGame.Core
```

## 🎯 优势

### ✅ 模块化设计
- 清晰的职责分离
- 易于维护和扩展
- 可独立开发和测试

### ✅ 测试友好
- 专门的测试项目
- 单元测试和集成测试分离
- 代码覆盖率支持

### ✅ 部署灵活
- 可独立部署各个组件
- 支持不同的部署策略
- 便于容器化

### ✅ 开发效率
- 清晰的目录结构
- 自动化脚本
- 标准化的构建流程 