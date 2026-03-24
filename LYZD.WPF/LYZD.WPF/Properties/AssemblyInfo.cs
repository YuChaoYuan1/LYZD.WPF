using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("LYZD.WPF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LYZD.WPF")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

//若要开始生成可本地化的应用程序，请设置
//.csproj 文件中的 <UICulture>CultureYouAreCodingWith</UICulture>
//例如，如果您在源文件中使用的是美国英语，
//使用的是美国英语，请将 <UICulture> 设置为 en-US。  然后取消
//对以下 NeutralResourceLanguage 特性的注释。  更新
//以下行中的“en-US”以匹配项目文件中的 UICulture 设置。

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //主题特定资源词典所处位置
                                     //(未在页面中找到资源时使用，
                                     //或应用程序资源字典中找到时使用)
    ResourceDictionaryLocation.SourceAssembly //常规资源词典所处位置
                                              //(未在页面中找到资源时使用，
                                              //、应用程序或任何主题专用资源字典中找到时使用)
)]


// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值
//通过使用 "*"，如下所示:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log\\Config\\log4net.config", ConfigFileExtension = "config", Watch = true)]

//一、版本确定为1 .0.0.1
//后续需要更新需要手动更新版本号
//版本号规则  　主版本号 . 子版本号 [.修正版本号 [.编译版本号]] 
//主版本号：为版本的大改动，对框架或代码的大量修改，不兼容之前版本
//子版本号：在原有基础上添加的部分功能，有可能与之前版本不兼容
//修正版本号：为代码小改动，完全兼容之前版本，没有冲突
//编译版本号：为特殊情况下给予特殊设备使用时的编译版本--如xp系统, win97系统下的特定修改

//二、版本号管理策略： 
//1．当项目在进行了局部修改或 bug 修正时 , 主版本号和子版本号都不变, 修正版本号加 1;
//2.当项目在原有的基础上增加了部分功能时 , 主版本号不变 , 子版本号加 1, 修正版本号复位为 0, 因而可以被忽略掉;
//3．当项目在进行了重大修改或局部修正累积较多 , 而导致项目整体发生全局变化时 , 主版本号加 1;

//三 、
//1：凡是进行子版本号或主版本号更新，需要实现与同项目管理的同事打声招呼，进行确定后在进行修改
//2：凡是版本更新，必须写更新文档，并存储到程序【帮助文档】文件夹中的版本更新备注中
//3：编写格式为
//修改时间
//修改人--可不用姓名-用个编号也行
//修改原因
//修改内容--需要详细到那个类库-那个方法-哪一行的那个属性
//修改较多情况需要附上修改截图--保存到git上，方便后续查问题