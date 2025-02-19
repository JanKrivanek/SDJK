using SCKRM;
using SCKRM.Input;
using UnityEngine;

namespace SDJK.MainMenu.MapSelectScreen
{
    public class MapSelectScreen : SCKRM.UI.UIBase
    {
        float upTimer = 0;
        float upTimer2 = 0;
        float downTimer = 0;
        float downTimer2 = 0;
        float leftTimer = 0;
        float leftTimer2 = 0;
        float rightTimer = 0;
        float rightTimer2 = 0;
        void Update()
        {
            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                bool up = ReapeatInput(KeyCode.UpArrow, ref upTimer, ref upTimer2);
                bool down = ReapeatInput(KeyCode.DownArrow, ref downTimer, ref downTimer2);
                bool left = ReapeatInput(KeyCode.LeftArrow, ref leftTimer, ref leftTimer2);
                bool right = ReapeatInput(KeyCode.RightArrow, ref rightTimer, ref rightTimer2);

                bool ReapeatInput(KeyCode keyCode, ref float timer, ref float timer2)
                {
                    if (InputManager.GetKey(keyCode))
                        return true;
                    else if (InputManager.GetKey(keyCode, InputType.Alway))
                    {
                        if (timer >= 0.25f)
                        {
                            if (timer2 >= 0.05f)
                            {
                                timer2 = 0;
                                return true;
                            }
                            else
                                timer2 += Kernel.unscaledDeltaTime;

                            return false;
                        }
                        else
                            timer += Kernel.unscaledDeltaTime;

                        return false;
                    }
                    else
                    {
                        timer2 = 0;
                        timer = 0;

                        return false;
                    }
                }

                if (up || left)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        MapManager.RulesetBackMapPack();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                        MapManager.RulesetBackMap();
                }
                if (down || right)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        MapManager.RulesetNextMapPack();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                        MapManager.RulesetNextMap();
                }
            }
        }
    }
}
