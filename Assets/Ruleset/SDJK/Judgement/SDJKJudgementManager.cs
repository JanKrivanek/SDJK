using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using SDJK.Ruleset.SDJK.GameOver;
using SDJK.Replay.Ruleset.SDJK;
using SDJK.Map;
using SDJK.Mode.Fun;
using SDJK.Effect.PostProcessing;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : JudgementManagerBase
    {
        public static new SDJKJudgementManager instance => (SDJKJudgementManager)JudgementManagerBase.instance;

        [SerializeField] SDJKManager _sdjkManager; public SDJKManager sdjkManager => _sdjkManager;
        [SerializeField] SDJKInputManager _inputManager; public SDJKInputManager inputManager => _inputManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;

        [SerializeField] HPVignette _hpVignette; public HPVignette hpVignette => _hpVignette;

        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;



        /// <summary>
        /// lastJudgementIndex[keyIndex]
        /// </summary>
        public List<int> lastJudgementIndex { get; } = new List<int>();
        /// <summary>
        /// lastAutoJudgementIndex[keyIndex]
        /// </summary>
        public List<int> lastAutoJudgementIndex { get; } = new List<int>();

        /// <summary>
        /// lastJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastJudgementBeat { get; } = new List<double>();
        /// <summary>
        /// lastAutoJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastAutoJudgementBeat { get; } = new List<double>();



        public List<Action> pressAction = new List<Action>();
        public List<Action> pressUpAction = new List<Action>();



        public JudgementAction judgementAction;
        public delegate void JudgementAction(double disSecond, bool isMiss, double accuracy, double generousAcccuracy, JudgementMetaData metaData);



        List<JudgementObject> judgements = new List<JudgementObject>();
        List<HoldJudgementObject> holdJudgements = new List<HoldJudgementObject>();
        public override bool Refresh()
        {
            if (base.Refresh())
            {
                RhythmManager.timeChanged -= TimeChanged;
                RhythmManager.timeChanged += TimeChanged;

                judgements.Clear();

                lastJudgementIndex.Clear();
                lastAutoJudgementIndex.Clear();

                lastJudgementBeat.Clear();
                lastAutoJudgementBeat.Clear();

                for (int i = 0; i < map.notes.Count; i++)
                {
                    judgements.Add(new JudgementObject(inputManager, map, i, lastJudgementIndex, lastJudgementBeat, false));
                    judgements.Add(new JudgementObject(inputManager, map, i, lastAutoJudgementIndex, lastAutoJudgementBeat, true));

                    lastJudgementIndex.Add(int.MinValue);
                    lastAutoJudgementIndex.Add(int.MinValue);

                    lastJudgementBeat.Add(double.MinValue);
                    lastAutoJudgementBeat.Add(double.MinValue);

                    pressAction.Add(null);
                    pressUpAction.Add(null);

                    if (!sdjkManager.isReplay)
                    {
                        sdjkManager.createdReplay.pressBeat.Add(new List<double>());
                        sdjkManager.createdReplay.pressUpBeat.Add(new List<double>());
                    }
                }

                return true;
            }

            return false;
        }

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                //게임 오버 상태 또는 일시정지 상태에선 판정하면 안됩니다
                if (!gameOverManager.isGameOver && !sdjkManager.isPaused && Kernel.gameSpeed != 0)
                {
                    for (int i = 0; i < judgements.Count; i++)
                        judgements[i].Update();

                    for (int i = 0; i < holdJudgements.Count; i++)
                        holdJudgements[i].Update();
                }

                if (RhythmManager.time >= 0 && RhythmManager.time < RhythmManager.length && !RhythmManager.isPaused)
                    health += map.globalEffect.hpAddValue.GetValue(RhythmManager.currentBeatSound) * RhythmManager.bpmDeltaTime;

                hpVignette.hp = (float)health;
                hpVignette.maxHp = (float)maxHealth;
            }
        }

        void OnEnable()
        {
            if (sdjkManager.isReplay)
            {
                RhythmManager.timeChanged -= TimeChanged;
                RhythmManager.timeChanged += TimeChanged;
            }
        }
        void OnDisable() => RhythmManager.timeChanged -= TimeChanged;

        public override void TimeChanged()
        {
            SDJKManager manager = sdjkManager;
            double currentBeat = RhythmManager.currentBeatSound;

            if (!manager.isReplay)
                return;

            for (int i = 0; i < judgements.Count; i++)
            {
                JudgementObject judgement = judgements[i];
                TypeList<SDJKNoteFile> notes = map.notes[judgement.keyIndex];

                int noteIndex = map.NoteBinarySearch(currentBeat, judgement.keyIndex, x => x.type != SDJKNoteTypeFile.instantDeath && x.type != SDJKNoteTypeFile.auto) - 1;
                int autoNoteIndex = map.NoteBinarySearch(currentBeat, judgement.keyIndex, x => x.type == SDJKNoteTypeFile.auto) - 1;

                judgement.currentNoteIndex = noteIndex;
                judgement.NextNote();

                int pressIndex = manager.currentReplay.PressBinarySearch(currentBeat, judgement.keyIndex) - 1;
                int pressUpIndex = manager.currentReplay.PressUpBinarySearch(currentBeat, judgement.keyIndex) - 1;

                judgement.currentPressBeatReplayIndex = pressIndex;
                judgement.currentPressUpBeatReplayIndex = pressUpIndex;

                judgement.NextPressBeatReplay();
                judgement.NextPressUpBeatReplay();

                lastJudgementBeat[judgement.keyIndex] = double.MinValue;
                lastJudgementIndex[judgement.keyIndex] = 0;

                lastAutoJudgementBeat[judgement.keyIndex] = double.MinValue;
                lastAutoJudgementIndex[judgement.keyIndex] = 0;
            }
        }

        public static void HitsoundPlay(TypeList<HitsoundFile> hitsoundFiles)
        {
            SDJKManager manager = instance.sdjkManager;
            for (int i = 0; i < hitsoundFiles.Count; i++)
                manager.HitsoundPlay(hitsoundFiles[i]);
        }

        class JudgementObject
        {
            public JudgementObject(SDJKInputManager inputManager, SDJKMapFile map, int keyIndex, List<int> lastJudgementIndex, List<double> lastJudgementBeat, bool autoNote)
            {
                this.inputManager = inputManager;
                this.map = map;
                this.keyIndex = keyIndex;
                this.lastJudgementIndex = lastJudgementIndex;
                this.lastJudgementBeat = lastJudgementBeat;
                this.autoNote = autoNote;

                NextNote();

                if (instance.sdjkManager.isReplay)
                {
                    NextPressBeatReplay();
                    NextPressUpBeatReplay();
                }

                TypeList<SDJKNoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                    currentNote = notes[currentNoteIndex];

                for (int i = 0; i < notes.Count; i++)
                {
                    SDJKNoteFile note = notes[i];
                    if (note.type == SDJKNoteTypeFile.instantDeath)
                        instantDeathNoteBeats.Add(note.beat);
                }
            }

            public SDJKInputManager inputManager;
            public SDJKMapFile map;
            public int keyIndex = 0;
            public bool autoNote;

            public SDJKNoteFile currentNote;
            public double currentPressBeatReplay;
            public double currentPressUpBeatReplay;
            public int currentNoteIndex = -1;
            public int currentPressBeatReplayIndex = -1;
            public int currentPressUpBeatReplayIndex = -1;

            /// <summary>
            /// lastJudgementBeat[keyIndex]
            /// </summary>
            List<int> lastJudgementIndex;
            List<double> lastJudgementBeat;

            List<double> instantDeathNoteBeats = new List<double>();

            bool normalInput = false;
            public void Update()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                TypeList<SDJKNoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;
                double currentBeat = RhythmManager.currentBeatSound;
                bool hitsoundPlay = false;

                TypeList<HitsoundFile> hitsoundFiles = currentNote.hitsoundFiles;
                SetDisSecond(currentNote.beat, true, out double realDisSecond, out double judgementDisSecond, currentPressBeatReplay);

                if (autoNote)
                {
                    if (currentNoteIndex < notes.Count && currentBeat >= currentNote.beat)
                    {
                        Input();
                        hitsoundPlay = true;
                    }
                }
                else if (sdjkManager.isReplay && keyIndex < sdjkManager.currentReplay.pressBeat.Count && keyIndex < sdjkManager.currentReplay.pressUpBeat.Count)
                {
                    List<double> pressBeats = sdjkManager.currentReplay.pressBeat[keyIndex];
                    List<double> pressUpBeats = sdjkManager.currentReplay.pressUpBeat[keyIndex];

                    while (currentPressBeatReplayIndex < pressBeats.Count && currentBeat >= currentPressBeatReplay)
                    {
                        instance.pressAction[keyIndex]?.Invoke();

                        double pressUpBeatReplay = currentPressBeatReplay;
                        if (currentPressBeatReplayIndex < pressUpBeats.Count)
                            pressUpBeatReplay = pressUpBeats[currentPressBeatReplayIndex];

                        Input(pressUpBeatReplay);
                        NextPressBeatReplay();

                        hitsoundPlay = true;
                    }

                    while (currentPressUpBeatReplayIndex < pressUpBeats.Count && currentBeat >= currentPressUpBeatReplay)
                    {
                        instance.pressUpAction[keyIndex]?.Invoke();
                        NextPressUpBeatReplay();
                    }
                }
                else
                {
                    if (inputManager.GetKey(keyIndex))
                    {
                        sdjkManager.createdReplay.pressBeat[keyIndex].Add(currentBeat);
                        instance.pressAction[keyIndex]?.Invoke();

                        Input();
                        hitsoundPlay = true;

                        normalInput = true;
                    }
                    //일시정지 상태가 해제되었을때 키를 누르지 않으면 바로 판정이 일어나야함
                    else if (normalInput && !inputManager.GetKey(keyIndex, InputType.Alway))
                    {
                        sdjkManager.createdReplay.pressUpBeat[keyIndex].Add(currentBeat);
                        instance.pressUpAction[keyIndex]?.Invoke();

                        normalInput = false;
                    }
                }

                for (int i = currentNoteIndex; realDisSecond >= missSecond && i < notes.Count; i++)
                {
                    Input(currentPressUpBeatReplay);
                    SetDisSecond(currentNote.beat, true, out realDisSecond, out judgementDisSecond, currentPressBeatReplay);
                }

                if (hitsoundPlay)
                    HitsoundPlay(hitsoundFiles);

                void Input(double pressUpBeatReplay = 0)
                {
                    if (currentNoteIndex >= notes.Count)
                        return;

                    if (Judgement(currentNote.beat, judgementDisSecond, false, out JudgementMetaData metaData, currentPressBeatReplay))
                    {
                        bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                        if (currentNote.holdLength > 0)
                        {
                            double holdBeat = currentNote.beat + currentNote.holdLength;

                            if (!isMiss) //미스가 아닐경우 홀드 노트 진행
                            {
                                HoldJudgementObject holdJudgementObject = new HoldJudgementObject(this, inputManager, map, keyIndex, autoNote, currentNote, holdBeat, pressUpBeatReplay);
                                instance.holdJudgements.Add(holdJudgementObject);

                                holdJudgementObject.Update();
                            }
                            else //미스 일경우 홀드 노트 패스
                                lastJudgementBeat[keyIndex] = lastJudgementBeat[keyIndex].Max(holdBeat);
                        }

                        lastJudgementIndex[keyIndex] = currentNoteIndex;
                        NextNote();
                    }
                }
            }

            public void SetDisSecond(double beat, bool maxClamp, out double realDisSecond, out double judgementDisSecond, double pressBeatReplay)
            {
                SDJKManager manager = instance.sdjkManager;
                double currentBeat;
                if (manager.isReplay)
                    currentBeat = pressBeatReplay;
                else
                    currentBeat = RhythmManager.currentBeatSound;

                realDisSecond = GetDisSecond(beat, maxClamp, currentBeat);
                judgementDisSecond = realDisSecond;

                if (autoNote)
                    judgementDisSecond = 0;
            }

            public bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData, double notePressBeatReplay)
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                TypeList<SDJKNoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;
                double bpmDivide60 = RhythmManager.bpm / 60d;

                if (ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                {
                    lastJudgementBeat[keyIndex] = lastJudgementBeat[keyIndex].Max(beat);
                    bool isMiss = metaData.nameKey == SDJKRuleset.miss;

                    //정확도
                    double accuracy;
                    {
                        if (sdjkManager.modes.FindMode<PinpointAccuracyModeBase>() != null)
                            accuracy = ruleset.GetAccuracy(disSecond);
                        else
                            accuracy = ruleset.GetGenerousAccuracy(disSecond, metaData);

                        instance.accuracyAbsList.Add(accuracy.Abs());
                        instance.accuracyAbs = instance.accuracyAbsList.Average();

                        instance.accuracys.Add(accuracy);
                        instance.accuracy = instance.accuracys.Average();

                        instance.rankProgress = instance.accuracyAbs;
                    }

                    if (!isMiss)
                    {
                        //콤보
                        instance.combo += 1;

                        double comboMultiplier = 0.75;
                        {
                            IMode comboMultiplierMode;
                            if ((comboMultiplierMode = sdjkManager.modes.FindMode<ComboMultiplierModeBase>()) != null)
                                comboMultiplier = (float)((ComboMultiplierModeBase.Config)comboMultiplierMode.modeConfig).multiplier;
                        }

                        //점수
                        {
                            instance.score += JudgementUtility.GetScoreAddValue(accuracy.Abs(), map.allJudgmentBeat.Count, instance.combo, comboMultiplier);

                            if (instance.maxCombo < instance.combo)
                            {
                                instance.maxCombo = instance.combo;

                                if (!sdjkManager.isReplay)
                                    sdjkManager.createdReplay.maxCombo.Add(currentBeat, instance.maxCombo);
                            }
                        }
                    }
                    else
                    {
                        instance.combo = 0;

                        IMode mode;
                        if (((mode = sdjkManager.modes.FindMode<AccelerationModeBase>()) != null && ((AccelerationModeBase.Config)mode.modeConfig).resetIfMiss) ||
                            ((mode = sdjkManager.modes.FindMode<DecelerationModeBase>()) != null && ((DecelerationModeBase.Config)mode.modeConfig).resetIfMiss))
                            sdjkManager.accelerationDeceleration = 1;
                    }

                    //체력
                    if (isMiss || metaData.missHp)
                        instance.health -= map.globalEffect.hpMissValue.GetValue(beat) * metaData.hpMultiplier;
                    else
                        instance.health += map.globalEffect.hpAddValue.GetValue(beat) * metaData.hpMultiplier;

                    //서든 데스 및 완벽주의자 모드
                    {
                        IMode mode;
                        if (isMiss && (mode = sdjkManager.modes.FindMode<SuddenDeathModeBase>()) != null)
                        {
                            SuddenDeathModeBase.Config config = (SuddenDeathModeBase.Config)mode.modeConfig;
                            if (config.restartOnFail)
                                sdjkManager.Restart();
                            else
                                instance.health = 0;
                        }
                        else if (accuracy != 0 && (mode = sdjkManager.modes.FindMode<PerfectModeBase>()) != null)
                        {
                            PerfectModeBase.Config config = (PerfectModeBase.Config)mode.modeConfig;
                            if (config.restartOnFail)
                                sdjkManager.Restart();
                            else
                                instance.health = 0;
                        }
                    }

                    //게임 오버
                    if (!sdjkManager.isReplay)
                    {
                        if (instance.health <= 0 && metaData.allowGameOver)
                            instance.gameOverManager.GameOver();
                    }
                    else if (currentBeat >= sdjkManager.currentReplay.gameOverBeat)
                        instance.gameOverManager.GameOver();

                    if (!sdjkManager.isReplay)
                        CreatedReplayFileAdd();
                    else
                        GetReplayFileValue();

                    instance.judgementAction?.Invoke(disSecond, isMiss, ruleset.GetAccuracy(disSecond), accuracy, metaData);
                    return true;
                }
                else if (instantDeathNoteBeats.Count > 0) //즉사 노트가 존재하며 노트를 치지 않았을때
                {
                    //가장 가까운 즉사 노트 감지
                    double instantDeathNoteBeat = instantDeathNoteBeats.CloseValue(currentBeat);
                    double instantDisSecond;
                    if (sdjkManager.isReplay)
                        instantDisSecond = GetDisSecond(instantDeathNoteBeat, false, notePressBeatReplay);
                    else
                        instantDisSecond = GetDisSecond(instantDeathNoteBeat, false, currentBeat);

                    if (instantDisSecond.Abs() <= missSecond)
                    {
                        instance.combo = 0;
                        instance.health = 0;

                        if (!sdjkManager.isReplay)
                        {
                            if (instance.health <= 0)
                                instance.gameOverManager.GameOver();
                        }
                        else if (currentBeat >= sdjkManager.currentReplay.gameOverBeat)
                            instance.gameOverManager.GameOver();

                        if (!sdjkManager.isReplay)
                            CreatedReplayFileAdd();
                        else
                            GetReplayFileValue();

                        double accuracy;
                        if (sdjkManager.modes.FindMode<PinpointAccuracyModeBase>() != null)
                            accuracy = ruleset.GetAccuracy(instantDisSecond);
                        else
                            accuracy = ruleset.GetGenerousAccuracy(instantDisSecond, metaData);

                        instance.judgementAction?.Invoke(instantDisSecond, true, ruleset.GetAccuracy(instantDisSecond), ruleset.GetGenerousAccuracy(instantDisSecond, metaData), ruleset.instantDeathJudgementMetaData);
                    }
                }

                return false;

                static void CreatedReplayFileAdd()
                {
                    SDJKManager sdjkManager = instance.sdjkManager;
                    double currentBeat = RhythmManager.currentBeatSound;

                    sdjkManager.createdReplay.combos.Add(currentBeat, instance.combo);
                    sdjkManager.createdReplay.maxCombo.Add(currentBeat, instance.maxCombo);
                    sdjkManager.createdReplay.scores.Add(currentBeat, instance.score);
                    sdjkManager.createdReplay.healths.Add(currentBeat, instance.health);
                    sdjkManager.createdReplay.accuracyAbses.Add(currentBeat, instance.accuracyAbs);
                    sdjkManager.createdReplay.accuracys.Add(currentBeat, instance.accuracy);
                    sdjkManager.createdReplay.rankProgresses.Add(currentBeat, instance.rankProgress);
                }

                static void GetReplayFileValue()
                {
                    SDJKManager sdjkManager = instance.sdjkManager;
                    SDJKReplayFile replay = sdjkManager.currentReplay;
                    double currentBeat = RhythmManager.currentBeatSound;

                    if (replay.combos.Count > 0)
                        instance.combo = replay.combos.GetValue(currentBeat);
                    if (replay.maxCombo.Count > 0)
                        instance.maxCombo = replay.maxCombo.GetValue(currentBeat);
                    if (replay.scores.Count > 0)
                        instance.score = replay.scores.GetValue(currentBeat);
                    if (replay.healths.Count > 0)
                        instance.health = replay.healths.GetValue(currentBeat);
                    if (replay.accuracyAbses.Count > 0)
                        instance.accuracyAbs = replay.accuracyAbses.GetValue(currentBeat);
                    if (replay.accuracys.Count > 0)
                        instance.accuracy = replay.accuracys.GetValue(currentBeat);
                    if (replay.rankProgresses.Count > 0)
                        instance.rankProgress = replay.rankProgresses.GetValue(currentBeat);
                }
            }

            public double GetDisSecond(double beat, bool maxClamp, double currentBeat)
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double bpmDivide60 = RhythmManager.bpm / 60d;
                double value = (currentBeat - beat) / bpmDivide60 / (RhythmManager.speed * Kernel.gameSpeed);
                if (maxClamp)
                    return value.Clamp(double.MinValue, missSecond);
                else
                    return value;
            }

            public void NextNote()
            {
                TypeList<SDJKNoteFile> notes = map.notes[keyIndex];

                for (int i = currentNoteIndex; i <= notes.Count; i++)
                {
                    currentNoteIndex++;

                    if (currentNoteIndex < notes.Count)
                    {
                        currentNote = notes[currentNoteIndex];

                        if (autoNote)
                        {
                            if (currentNote.type == SDJKNoteTypeFile.auto)
                                break;
                        }
                        else
                        {
                            if (currentNote.type != SDJKNoteTypeFile.instantDeath && currentNote.type != SDJKNoteTypeFile.auto)
                                break;
                        }
                    }
                }
            }

            public void NextPressBeatReplay()
            {
                SDJKManager sdjkManager = instance.sdjkManager;

                if (keyIndex < sdjkManager.currentReplay.pressBeat.Count)
                {
                    List<double> pressBeatReplay = sdjkManager.currentReplay.pressBeat[keyIndex];

                    currentPressBeatReplayIndex++;
                    if (currentPressBeatReplayIndex < pressBeatReplay.Count)
                        currentPressBeatReplay = pressBeatReplay[currentPressBeatReplayIndex];
                    else
                        currentPressBeatReplay = double.MaxValue;
                }
            }

            public void NextPressUpBeatReplay()
            {
                SDJKManager sdjkManager = instance.sdjkManager;

                if (keyIndex < sdjkManager.currentReplay.pressUpBeat.Count)
                {
                    List<double> pressUpBeatReplay = sdjkManager.currentReplay.pressUpBeat[keyIndex];

                    currentPressUpBeatReplayIndex++;
                    if (currentPressUpBeatReplayIndex < pressUpBeatReplay.Count)
                        currentPressUpBeatReplay = pressUpBeatReplay[currentPressUpBeatReplayIndex];
                    else
                        currentPressUpBeatReplay = double.MaxValue;
                }
            }
        }

        class HoldJudgementObject
        {
            public HoldJudgementObject(JudgementObject judgementObject, SDJKInputManager inputManager, SDJKMapFile map, int keyIndex, bool autoNote, SDJKNoteFile currentNote, double currentHoldNoteBeat, double replayInputBeat)
            {
                this.judgementObject = judgementObject;
                this.inputManager = inputManager;
                this.map = map;

                this.keyIndex = keyIndex;
                this.autoNote = autoNote;

                this.currentNote = currentNote;
                this.currentHoldNoteBeat = currentHoldNoteBeat;
                this.replayInputBeat = replayInputBeat;
            }

            JudgementObject judgementObject;
            SDJKInputManager inputManager;
            SDJKMapFile map;

            int keyIndex = 0;
            bool autoNote;

            SDJKNoteFile currentNote;
            double currentHoldNoteBeat;
            double replayInputBeat;

            bool isRemove = false;

            public void Update()
            {
                if (isRemove)
                    return;

                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;

                bool input;
                if (autoNote)
                    input = currentBeat <= currentHoldNoteBeat;
                else if (sdjkManager.isReplay)
                    input = currentBeat <= replayInputBeat;
                else
                    input = inputManager.GetKey(keyIndex, InputType.Alway);

                judgementObject.SetDisSecond(currentHoldNoteBeat, true, out double realDisSecond, out double judgementDisSecond, replayInputBeat);

                if (realDisSecond >= missSecond || !input)
                {
                    judgementObject.Judgement(currentHoldNoteBeat, judgementDisSecond, true, out _, replayInputBeat);
                    HitsoundPlay(currentNote.holdHitsoundFiles);

                    instance.holdJudgements.Remove(this);
                    isRemove = true;
                }
            }
        }
    }
}
