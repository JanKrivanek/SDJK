using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.NBS;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using SDJK.Mode.Fun;
using System.IO;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class BGM : ObjectPoolingBase
    {
        public ISoundPlayer soundPlayer { get; private set; }

        public float volumePade { get; private set; } = 0;
        public bool padeOut { get; set; } = false;

        public bool isLoaded { get; private set; } = false;



        AudioClip audioClip;
        MapFile map;
        public async UniTaskVoid Refresh(MapPack lastMapPack, double lastTime)
        {
            isLoaded = false;
            map = MapManager.selectedMap;

            string path = PathUtility.Combine(map.mapFilePathParent, map.info.songFile);
            if (ResourceManager.FileExtensionExists(path, out string fullPath, ResourceManager.audioExtension))
            {
                audioClip = await ResourceManager.GetAudio(fullPath, true, false, HideFlags.DontSave, true);

                if (!Kernel.isPlaying || this == null)
                {
                    AudioDestroy();
                    return;
                }

                SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, audioClip);
                SoundData<SoundMetaData> soundData = ResourceManager.CreateSoundData("", true, soundMetaData);

                soundPlayer = SoundManager.PlaySound(soundData, 0);

                RhythmManager.SoundPlayerChange(soundPlayer);
                isLoaded = true;
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                soundPlayer = SoundManager.PlayNBS(soundData, 0);

                RhythmManager.SoundPlayerChange(soundPlayer);
                isLoaded = true;
            }
            else
                Remove();

            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                if (lastMapPack != MapManager.selectedMapPack)
                    RhythmManager.internalTime = (float)map.info.mainMenuStartTime;
                else
                    RhythmManager.time = lastTime;
            }
        }

        double accelerationDeceleration = 1;
        void Update()
        {
            if (!isLoaded || ResourceManager.isAudioReset)
                return;

            if (!padeOut && RhythmManager.isPlaying && (soundPlayer == null || soundPlayer.IsDestroyed() || soundPlayer.isRemoved))
                SoundPlayerRemoved();

            if (padeOut)
            {
                soundPlayer.volume = soundPlayer.volume.MoveTowards(0, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (soundPlayer.volume <= 0)
                {
                    Remove();
                    return;
                }
            }
            else
            {
                volumePade = volumePade.MoveTowards(1, 0.05f * Kernel.fpsUnscaledSmoothDeltaTime);

                soundPlayer.volume = (float)map.globalEffect.volume.GetValue(RhythmManager.currentBeatSound) * volumePade;
                soundPlayer.pitch = (float)map.globalEffect.pitch.GetValue(RhythmManager.currentBeatSound);

                RhythmManager.speed = (float)map.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound);

                //모드
                {
                    IMode mode;
                    if ((mode = ModeManager.selectedModeList.FindMode<FastModeBase>()) != null)
                    {
                        RhythmManager.speed *= (float)((FastModeBase.Config)mode.modeConfig).speed;
                        soundPlayer.pitch *= (float)((FastModeBase.Config)mode.modeConfig).pitch;
                    }
                    else if ((mode = ModeManager.selectedModeList.FindMode<SlowModeBase>()) != null)
                    {
                        RhythmManager.speed *= (float)((SlowModeBase.Config)mode.modeConfig).speed;
                        soundPlayer.pitch *= (float)((SlowModeBase.Config)mode.modeConfig).pitch;
                    }

                    if (RhythmManager.currentBeatSound >= 0)
                    {
                        if ((mode = ModeManager.selectedModeList.FindMode<AccelerationModeBase>()) != null)
                        {
                            AccelerationModeBase.Config config = (AccelerationModeBase.Config)mode.modeConfig;

                            accelerationDeceleration += config.coefficient * Kernel.deltaTimeDouble;
                            accelerationDeceleration = accelerationDeceleration.Clamp(0, config.max);

                            RhythmManager.speed *= accelerationDeceleration;
                            if (config.changePitch)
                                soundPlayer.pitch *= (float)accelerationDeceleration;
                        }
                        else if ((mode = ModeManager.selectedModeList.FindMode<DecelerationModeBase>()) != null)
                        {
                            DecelerationModeBase.Config config = (DecelerationModeBase.Config)mode.modeConfig;

                            accelerationDeceleration -= config.coefficient * Kernel.deltaTimeDouble;
                            accelerationDeceleration = accelerationDeceleration.Clamp(config.min);

                            RhythmManager.speed *= accelerationDeceleration;
                            if (config.changePitch)
                                soundPlayer.pitch *= (float)accelerationDeceleration;
                        }
                        else
                            accelerationDeceleration = 1;
                    }
                }
            }
        }

        void SoundPlayerRemoved()
        {
            AudioDestroy();

            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
                Refresh(null, 0).Forget();
            else
                MapManager.RulesetNextMapPack();
        }

        public override void Remove()
        {
            base.Remove();

            if (soundPlayer != null && !soundPlayer.IsDestroyed() && !soundPlayer.isRemoved)
                soundPlayer.Remove();

            isLoaded = false;

            soundPlayer = null;

            volumePade = 0;
            padeOut = false;

            AudioDestroy();

            audioClip = null;
            accelerationDeceleration = 1;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AudioDestroy();
        }

        void AudioDestroy()
        {
            if (audioClip != null)
                Destroy(audioClip, 1);
        }
    }
}
