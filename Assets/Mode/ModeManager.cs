using SCKRM;
using SCKRM.Reflection;
using SCKRM.Renderer;
using SCKRM.SaveLoad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDJK.Mode
{
    public static class ModeManager
    {
        public static List<IMode> modeList { get; } = new List<IMode>();
        public static List<IMode> selectedModeList { get; } = new List<IMode>();

        public static event Action isModeRefresh;
        public static bool isModeRefreshEnd { get; private set; } = false;

        [Starten]
        public static void ModeListRefresh()
        {
            Type[] types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
            {
                Type type = types[typesIndex];
                if (type.IsPublic && type.IsClass && !type.IsAbstract && !type.IsSpecialName)
                {
                    Type[] interfaces = type.GetInterfaces();
                    for (int interfaceIndex = 0; interfaceIndex < interfaces.Length; interfaceIndex++)
                    {
                        Type interfaceType = interfaces[interfaceIndex];
                        if (interfaceType == typeof(IMode))
                        {
                            modeList.Add((IMode)Activator.CreateInstance(type));
                            break;
                        }
                    }
                }
            }

            //정렬
            {
                List<IMode> elements = modeList.OrderBy(x => x.order).ToList();

                modeList.Clear();
                modeList.AddRange(elements);
            }

            selectedModeList.Clear();
            isModeRefreshEnd = true;
            isModeRefresh?.Invoke();
        }

        /// <summary>
        /// 선택한 규칙 집합이랑 호환되는 모드인지 확인합니다
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="targetModeNames"></param>
        /// <returns></returns>
        [WikiDescription("선택한 규칙 집합이랑 호환되는 모드인지 확인합니다")]
        public static bool IsCompatibleMode(this IMode mode, string targetRulesetName)
        {
            if (mode.targetRuleset == targetRulesetName)
                return true;

            return false;
        }

        public static IMode FindMode(string name) => modeList.Find(x => x.name == name);
        public static IMode FindSelectedMode(string name) => selectedModeList.Find(x => x.name == name);

        public static void SelectMode(string name) => SelectMode(FindMode(name));
        public static void SelectMode(IMode mode)
        {
            if (!selectedModeList.Contains(mode))
            {
                for (int i = 0; i < selectedModeList.Count; i++)
                {
                    IMode tempMode = selectedModeList[i];
                    if (mode.incompatibleModes.Contains(tempMode.name))
                        DeselectMode(tempMode);
                }

                selectedModeList.Add(mode);
            }
        }

        public static void DeselectMode(string name) => DeselectMode(FindMode(name));
        public static void DeselectMode(IMode mode) => selectedModeList.Remove(mode);
    }

    /// <summary>
    /// 이 인터페이스를 상속하면 SDJK가 모드를 자동으로 감지합니다
    /// </summary>
    [WikiDescription("이 인터페이스를 상속하면 SDJK가 모드를 자동으로 감지합니다")]
    public interface IMode
    {
        public NameSpacePathReplacePair title { get; }
        public int order { get; }

        public string name { get; }

        public NameSpacePathReplacePair displayName { get; }
        public NameSpacePathPair info { get; }

        public NameSpaceIndexTypePathPair icon { get; }

        public string targetRuleset { get; }
        public string[] incompatibleModes { get; }

        public IModeConfig modeConfig { get; }
        public SaveLoadClass modeConfigSlc { get; }

        public IModeConfig CreateModeConfig();
    }

    public interface IModeConfig { }

    public sealed class ModeConfigSaveLoadAttribute : SaveLoadBaseAttribute { }

    /// <summary>
    /// <see cref="IMode"/> 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다
    /// </summary>
    [WikiDescription("IMode 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다")]
    public abstract class ModeBase : IMode
    {
        public abstract NameSpacePathReplacePair title { get; }
        public abstract int order { get; }

        public abstract string name { get; }

        public abstract NameSpacePathReplacePair displayName { get; }
        public virtual NameSpacePathPair info => "";

        public abstract NameSpaceIndexTypePathPair icon { get; }

        public abstract string targetRuleset { get; }
        public virtual string[] incompatibleModes => null;

        public virtual IModeConfig modeConfig
        {
            get
            {
                if (_modeConfig == null)
                    _modeConfig = CreateModeConfig();

                return _modeConfig;
            }
        }
        IModeConfig _modeConfig = null;

        public SaveLoadClass modeConfigSlc
        {
            get
            {
                if (_modeConfigSlc == null)
                {
                    if (modeConfig == null)
                        return null;

                    SaveLoadManager.Initialize(modeConfig.GetType(), typeof(ModeConfigSaveLoadAttribute), out _modeConfigSlc);
                }

                return _modeConfigSlc;
            }
        }
        SaveLoadClass _modeConfigSlc;

        public virtual IModeConfig CreateModeConfig() => null;
    }
}
