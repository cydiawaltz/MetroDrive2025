using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BveTypes.ClassWrappers;
using FastMember;
using ObjectiveHarmonyPatch;
using TypeWrapping;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;

using System.IO.Pipes;
using System.IO;
using System.Threading;

using MapPlugin = MetroDrive.MapPlugin.PluginMain;

namespace MetroDrive.Extension//おーとまさん作「Automatic9045.AtsEx.StructureReuser」(未公開)
{
    [Plugin(PluginType.Extension)]
    [HideExtensionMain]
    //[Togglable]
    public class PluginMain : AssemblyPluginBase, IExtension
    {
        private static MethodInfo UnloadScenarioMethod = null;

        private readonly HarmonyLib.Harmony Harmony = new HarmonyLib.Harmony("com.structure-reuser");

        public bool IsEnabled { get; set; } = false;

        public PluginMain(PluginBuilder builder) : base(builder)
        {
            ClassMemberSet mainFormMembers = BveHacker.BveTypes.GetClassInfoOf<MainForm>();
            FastMethod openScenarioMethod = mainFormMembers.GetSourceMethodOf(nameof(MainForm.OpenScenario));
            FastMethod unloadScenarioMethod = mainFormMembers.GetSourceMethodOf(nameof(MainForm.UnloadScenario));

            UnloadScenarioMethod = unloadScenarioMethod.Source;
            HarmonyLib.HarmonyMethod transpilerMethod = new HarmonyLib.HarmonyMethod(typeof(PluginMain), nameof(Transpile));
            Harmony.Patch(openScenarioMethod.Source, transpiler: transpilerMethod);
            //以上元のコード
            BveHacker.MainFormSource.Shown += MainFormSource_Shown;
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.Out))
            {
                try
                {
                    pipeClient.Connect(1000);
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        writer.WriteLine("ready");
                        writer.Flush();
                    }
                }
                catch (TimeoutException)
                {
                    MessageBox.Show("接続がタイムアウトされました。ポーズメニューより「運転終了」を選択した上、ゲームを再起動してください。");
                }
            }
            OpenPipe();
            //シナリオの読み込みはUnity側の.batファイルでやる
            Plugins.AllPluginsLoaded += onAllPluginsLoaded;
        }
        void onAllPluginsLoaded(object sender, EventArgs e)
        {
            MapPlugin mapPlugin = Plugins[PluginType.MapPlugin][""] as MapPlugin;
        }
        async void OpenPipe()
        {
            using (var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut))
            {
                await pipeServer.WaitForConnectionAsync();
                try
                {
                    using (var reader = new StreamReader(pipeServer, Encoding.UTF8))
                    {
                        while (true)
                        {
                            string message = await reader.ReadLineAsync();
                            if (message == "end")
                            {
                                using (StreamWriter writer = new StreamWriter(pipeServer))
                                {
                                    writer.WriteLine("done");
                                    writer.Flush(); // これによりデータがパイプに即座に書き込まれる
                                }
                                break;
                            }
                            else
                            {
                                OpenScenario(message);
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("名前付きパイプ通信でエラーが発生しました。" + ex.Message);
                }
            }
        }

        private void MainFormSource_Shown(object sender, EventArgs e)//自作
        {
            //BveHacker.MainFormSource.Hide();
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            Harmony.UnpatchAll(Harmony.Id);
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            return new ExtensionTickResult();
        }
        void OpenScenario(string mes)//自作
        {
            string filedirectory = BveHacker.ScenarioInfo.DirectoryName;
            ScenarioInfo scenarioinfo = ScenarioInfo.FromFile(Path.Combine(filedirectory, (int.Parse(mes) - 10).ToString() + ".txt"));
            BveHacker.MainForm.LoadScenario(scenarioinfo, true);
            BveHacker.MainFormSource.Show();
        }

        private static IEnumerable<HarmonyLib.CodeInstruction> Transpile(IEnumerable<HarmonyLib.CodeInstruction> instructions)
        {
            List<HarmonyLib.CodeInstruction> list = instructions.ToList();

            int step = 0;
            for (int i = list.Count - 1; 0 <= i; i--)
            {
                HarmonyLib.CodeInstruction instruction = list[i];

                switch (step)
                {
                    case 0:
                        if (instruction.opcode == OpCodes.Ldc_I4_0)
                        {
                            instruction.opcode = OpCodes.Ldc_I4_1;
                            step++;
                        }
                        break;

                    case 1:
                        if (instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo operandMethod && operandMethod == UnloadScenarioMethod)
                        {
                            instruction.opcode = OpCodes.Nop;
                            step++;
                        }
                        break;

                    case 2:
                        if (instruction.opcode == OpCodes.Ldarg_0)
                        {
                            instruction.opcode = OpCodes.Nop;
                            return list;
                        }
                        break;
                }
            }

            throw new InvalidOperationException("削除対象のメソッドが見つかりませんでした。");
        }
    }
}
