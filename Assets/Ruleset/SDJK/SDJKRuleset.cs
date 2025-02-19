using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using SDJK.Map;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;
using SDJK.Replay;
using SDJK.Mode;
using System.Collections.Generic;
using SDJK.Mode.Automatic;
using SDJK.Replay.Ruleset.SDJK;
using SDJK.Ruleset.SDJK.GameOver;
using SCKRM;
using SDJK.Mode.Difficulty;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : RulesetBase
    {
        public override int order { get; } = int.MinValue;

        public override string name { get; } = "sdjk";
        public override string displayName { get; } = "SDJK";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");
        public override string discordIconKey => "ruleset_sdjk";

        public override string[] compatibleRulesets { get; } = new string[] { /*"super_hexagon",*/ "adofai", "osu!", "osu!mania" };

        public override RankMetaData[] rankMetaDatas { get; } = new RankMetaData[]
        {
            new RankMetaData("SS", 0, new Color32(255, 0, 255, 255)),
            new RankMetaData("S", 0.0625, new Color32(0, 220, 255, 255)),
            new RankMetaData("A", 0.125, new Color32(0, 170, 255, 255)),
            new RankMetaData("B", 0.25, new Color32(100, 255, 100, 255)),
            new RankMetaData("C", 0.5, new Color32(230, 230, 0, 255)),
            new RankMetaData("F", 0.9, new Color32(166, 166, 166, 255)),
            new RankMetaData("...", 1, new Color32(255, 255, 255, 255))
        };

        public override JudgementMetaData[] judgementMetaDatas { get; } = new JudgementMetaData[]
        {
            new JudgementMetaData(sick, 0.0142222222, new Color32(0, 220, 255, 255), 1),
            new JudgementMetaData(perfect, 0.0426666667, new Color32(0, 170, 255, 255), 0.0625, true, false),
            new JudgementMetaData(great, 0.0711111111, new Color32(100, 255, 100, 255), 0.125, true),
            new JudgementMetaData(good, 0.0995555555, new Color32(230, 230, 0, 255), 0.25, true),
            new JudgementMetaData(early, 0.128, new Color32(166, 166, 166, 255), 0.5, true)
        };
        public override JudgementMetaData missJudgementMetaData { get; } = new JudgementMetaData(miss, double.MaxValue, new Color32(255, 50, 50, 255), 1, true);
        public JudgementMetaData instantDeathJudgementMetaData { get; } = new JudgementMetaData(instantDeath, double.MaxValue, new Color32(255, 50, 50, 255), double.MaxValue, true);

        public const string sick = "ruleset.sdjk.sick";
        public const string perfect = "ruleset.sdjk.perfect";
        public const string great = "ruleset.sdjk.great";
        public const string good = "ruleset.sdjk.good";
        public const string early = "ruleset.sdjk.early";
        public const string miss = "ruleset.sdjk.miss";
        public const string instantDeath = "ruleset.sdjk.instantDeath";

        public override async void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            base.GameStart(mapFilePath, replayFilePath, isEditor, modes);

            SDJKMapFile map = null;
            SDJKReplayFile replay = null;

            await SceneManager.LoadScene(3, () => UniTask.RunOnThreadPool(MapLoad));
            await UniTask.NextFrame();

            void MapLoad()
            {
                replay = null;
                if (replayFilePath != null)
                    replay = ReplayLoader.ReplayLoad<SDJKReplayFile>(replayFilePath, out modes);

                map = MapLoader.MapLoad<SDJKMapFile>(mapFilePath, false, modes);

                if (modes.FindMode<AutoModeBase>() != null)
                    replay = GetAutoModeReplayFile(map, modes);
            }

            if (!Kernel.isPlaying)
                return;
#if UNITY_2023_1_OR_NEWER
            Object.FindFirstObjectByType<SDJKManager>(FindObjectsInactive.Include).Refresh(map, replay, this, isEditor, modes);
            Object.FindFirstObjectByType<SDJKInputManager>(FindObjectsInactive.Include).Refresh();
            Object.FindFirstObjectByType<SDJKJudgementManager>(FindObjectsInactive.Include).Refresh();
            Object.FindFirstObjectByType<SDJKGameOverManager>(FindObjectsInactive.Include).Refresh();
#else
            Object.FindObjectOfType<SDJKManager>(true).Refresh(map, replay, this, isEditor, modes);
            Object.FindObjectOfType<SDJKInputManager>(true).Refresh();
            Object.FindObjectOfType<SDJKJudgementManager>(true).Refresh();
            Object.FindObjectOfType<SDJKGameOverManager>(true).Refresh();
#endif

            //나중에 다시 필요할 수도...
            /*for (int i = 8; i < 135; i += 8)
            {
                for (int j = 0; j < 8; j++)
                    map.globalEffect.cameraPos.Add(i + j - 0.1, 0.25, new SCKRM.Json.JVector3(-0.8f + (j * 1.5f), 0, -14), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);

                map.globalEffect.cameraPos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(-0.8f, 0, -14));

                for (int j = 0; j < map.effect.fieldEffect.Count; j++)
                {
                    var field = map.effect.fieldEffect[j];
                    field.pos.Add(i + j, 0.1, new SCKRM.Json.JVector3(j * 1.5f, -1));
                    field.pos.Add(i + j + 0.1, 0.25, new SCKRM.Json.JVector3(j * 1.5f, 0), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);
                    field.pos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(j * 1.5f, 16));

                    if (j < map.effect.fieldEffect.Count - 1)
                    {
                        for (int k = 1; k < field.barEffect.Count; k++)
                        {
                            var bar = field.barEffect[k];
                            bar.color.Add(i + j + 1, 0.25, new SCKRM.Json.JColor(1, 1, 1, 0));
                            bar.color.Add(i + 7.4, 0, new SCKRM.Json.JColor(1, 1, 1, 1));

                            bar.noteColor.Add(i + j + 1, 0.25, new SCKRM.Json.JColor(1, 1, 1, 0));
                            bar.noteColor.Add(i + 7.4, 0, new SCKRM.Json.JColor(1, 1, 1, 1));
                        }
                    }
                }
            }

            for (int i = 0; i < map.effect.fieldEffect.Count; i++)
            {
                var field = map.effect.fieldEffect[i];
                for (int j = 0; j < field.barEffect.Count; j++)
                {
                    var bar = field.barEffect[j];
                    if (i == 1 && j != 0)
                    {
                        bar.pos.Add();
                        bar.height.Add();
                    }
                }
            }

            for (double i = 357.7; i < 421.5; i += 8)
            {
                for (int j = 0; j < 8; j++)
                    map.globalEffect.cameraPos.Add(i + j - 0.1, 0.25, new SCKRM.Json.JVector3(-0.8f + (j * 1.5f), 0, -14), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);

                map.globalEffect.cameraPos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(-0.8f, 0, -14));

                for (int j = 0; j < map.effect.fieldEffect.Count; j++)
                {
                    var field = map.effect.fieldEffect[j];

                    if (j != 1)
                    {
                        field.pos.Add(i + j, 0.1, new SCKRM.Json.JVector3(j * 1.5f, -1 + 8));
                        field.pos.Add(i + j + 0.1, 0.25, new SCKRM.Json.JVector3(j * 1.5f, 0 + 8), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);
                        field.pos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(j * 1.5f, 16 + 8));
                    }
                    else
                    {
                        field.pos.Add(i + j, 0.1, new SCKRM.Json.JVector3(j * 1.5f, -1));
                        field.pos.Add(i + j + 0.1, 0.25, new SCKRM.Json.JVector3(j * 1.5f, 0), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);
                        field.pos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(j * 1.5f, 16));
                    }

                    if (j < map.effect.fieldEffect.Count - 1)
                    {
                        for (int k = 1; k < field.barEffect.Count; k++)
                        {
                            var bar = field.barEffect[k];
                            if (j != 1)
                            {
                                bar.color.Add(i + j + 1, 0.25, new SCKRM.Json.JColor(1, 1, 1, 0));
                                bar.color.Add(i + 7.4, 0, new SCKRM.Json.JColor(1, 1, 1, 1));

                                bar.noteColor.Add(i + j + 1, 0.25, new SCKRM.Json.JColor(1, 1, 1, 0));
                                bar.noteColor.Add(i + 7.4, 0, new SCKRM.Json.JColor(1, 1, 1, 1));
                            }
                            else
                            {
                                bar.pos.Add(i + j + 1, 0.1, new SCKRM.Json.JVector3(0, -6.6255f, 0));
                                bar.pos.Add(i + j + 1.1, 0.25, new SCKRM.Json.JVector3(0, -5.878f, 0), SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);
                                bar.pos.Add(i + 7.4, 0, new SCKRM.Json.JVector3(0, 0, 0));

                                bar.height.Add(i + j + 1, 0.1, 0.448);
                                bar.height.Add(i + j + 1.1, 0.25, 0.5102f, SCKRM.Easing.EasingFunction.Ease.EaseInOutBack);
                                bar.height.Add(i + 7.4, 1, 1);
                            }
                        }
                    }
                }
            }*/

            /*System.Collections.Generic.List<SCKRM.Rhythm.BeatValuePairAni<double>> beats = new System.Collections.Generic.List<SCKRM.Rhythm.BeatValuePairAni<double>>();
            for (int i = 0; i < map.allJudgmentBeat[map.allJudgmentBeat.Count - 1]; i++)
            {
                if (map.globalEffect.yukiMode.GetValue(i))
                {
                    SCKRM.Rhythm.BeatValuePairAni<double> asdf = new SCKRM.Rhythm.BeatValuePairAni<double>() { beat = i, length = 0, value = 0.98f, easingFunction = SCKRM.Easing.EasingFunction.Ease.Linear };
                    map.globalEffect.cameraZoom.Add(asdf);
                    beats.Add(asdf);
                }
            }

            for (int i = 0; i < map.effect.fieldEffect[0].barEffect.Count; i++)
                map.effect.fieldEffect[0].barEffect[i].noteSpeed.Add(double.MinValue, 1);

            for (double i = 0; i < map.allJudgmentBeat[map.allJudgmentBeat.Count - 1]; i += 0.125f)
            {
                if (map.globalEffect.yukiMode.GetValue(i))
                {
                    for (int j = 0; j < map.notes.Count; j++)
                    {
                        SDJKNoteFile? note = map.notes[j].Find(x => SCKRM.MathTool.Distance(x.beat, i) <= SCKRM.MathTool.epsilonFloatWithAccuracy);
                        if (note != null)
                        {
                            SCKRM.Rhythm.BeatValuePairList<double> effect = map.effect.fieldEffect[0].barEffect[j].noteSpeed;
                            if (note.Value.holdLength > 0)
                            {
                                effect.Add(i, 0.5, true);
                                effect.Add(i + 0.125, 1, true);
                            }
                            else if (i >= 496 && i <= 560)
                            {
                                effect.Add(i, Random.Range(0.5f, 1.5f), true);
                                effect.Add(i + 0.125, 1, true);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < map.effect.fieldEffect[0].barEffect.Count; i++)
                map.effect.fieldEffect[0].barEffect[i].noteSpeed.Add(560, 1);

            map.globalEffect.cameraZoom.Sort((a, b) => a.beat.CompareTo(b.beat));

            for (int i = 0; i < beats.Count; i++)
            {
                SCKRM.Rhythm.BeatValuePairAni<double> asdf = new SCKRM.Rhythm.BeatValuePairAni<double>() { beat = beats[i].beat, length = 2, value = 1, easingFunction = SCKRM.Easing.EasingFunction.Ease.EaseOutExpo };
                map.globalEffect.cameraZoom.Insert(map.globalEffect.cameraZoom.IndexOf(beats[i]) + 1, asdf);
            }

            System.IO.File.WriteAllText(mapFilePath, SCKRM.Json.JsonManager.ObjectToJson(map));*/
        }

        static SDJKReplayFile GetAutoModeReplayFile(SDJKMapFile map, params IMode[] modes)
        {
            SDJKReplayFile replay = ReplayLoader.CreateReplay<SDJKReplayFile>(map, modes);
            double comboMultiplier = 0.75;
            {
                IMode comboMultiplierMode;
                if ((comboMultiplierMode = modes.FindMode<ComboMultiplierModeBase>()) != null)
                    comboMultiplier = (float)((ComboMultiplierModeBase.Config)comboMultiplierMode.modeConfig).multiplier;
            }

            int combo = 0;
            double score = 0;
            for (int i = 0; i < map.notes.Count; i++)
            {
                replay.pressBeat.Add(new List<double>());
                replay.pressUpBeat.Add(new List<double>());
            }

            for (int i = 0; i < map.allNotes.Count; i++)
            {
                SDJKAllNoteFile allNote = map.allNotes[i];
                SDJKNoteFile note = map.notes[allNote.keyIndex][allNote.index];
                if (note.type == SDJKNoteTypeFile.instantDeath || note.type == SDJKNoteTypeFile.auto)
                    continue;

                replay.pressBeat[allNote.keyIndex].Add(note.beat);
                replay.pressUpBeat[allNote.keyIndex].Add(note.beat + note.holdLength);

                combo++;
                score += JudgementUtility.GetScoreAddValue(0, map.allJudgmentBeat.Count, combo, comboMultiplier);

                replay.scores.Add(note.beat, score);
                replay.combos.Add(note.beat, combo);
                replay.maxCombo.Add(note.beat, combo);

                if (note.holdLength > 0)
                {
                    combo++;
                    score += JudgementUtility.GetScoreAddValue(0, map.allJudgmentBeat.Count, combo, comboMultiplier);

                    replay.scores.Add(note.beat + note.holdLength, score);
                    replay.combos.Add(note.beat + note.holdLength, combo);
                    replay.maxCombo.Add(note.beat + note.holdLength, combo);
                }
            }

            return replay;
        }
    }
}
