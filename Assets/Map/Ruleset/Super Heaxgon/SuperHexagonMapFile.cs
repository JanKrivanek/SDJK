using SCKRM.Easing;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Map.Ruleset.SuperHexagon.Map
{
    public sealed class SuperHexagonMapFile : MapFile
    {
        public SuperHexagonMapFile(string mapFilePath) : base(mapFilePath) { }

        /// <summary>
        /// notes[bar_index][note_index] = note
        /// </summary>
        public TypeList<TypeList<SuperHexagonNoteFile>> notes { get; set; } = new();

        public SuperHexagonEffectFile effect { get; set; } = new();

        public override TypeList<double> GetDifficulty() => ListDifficultyCalculation(allJudgmentBeat, 1.5);

        public override void FixAllJudgmentBeat()
        {
            TypeList<double> allJudgmentBeat = new TypeList<double>();

            for (int i = 0; i < notes.Count; i++)
            {
                TypeList<SuperHexagonNoteFile> notes = this.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    SuperHexagonNoteFile note = notes[j];

                    //모든 판정 비트에 노트 추가
                    allJudgmentBeat.Add(note.beat);
                    if (note.holdLength > 0)
                        allJudgmentBeat.Add(note.beat + note.holdLength);
                }
            }

            allJudgmentBeat.Sort();
            this.allJudgmentBeat = allJudgmentBeat;
        }

        public override void SetVisualizerEffect()
        {
            visualizerEffect.divide.Clear();
            for (int i = 0; i < effect.sidesList.Count; i++)
            {
                BeatValuePairAni<double> sides = effect.sidesList[i];
                visualizerEffect.divide.Add(sides.beat, 0, (int)sides.value);
            }
        }
    }

    public struct SuperHexagonNoteFile
    {
        public double beat { get; set; }
        public double holdLength { get; set; }

        public SuperHexagonNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }
    }

    public sealed class SuperHexagonEffectFile
    {
        public BeatValuePairAniListDouble sidesList { get; set; } = new(6);
        public BeatValuePairAniListDouble playerSpeed { get; set; } = new(12);

        public BeatValuePairAniListVector3 fieldPosition { get; set; } = new(Vector3.zero);
        public BeatValuePairAniListVector3 fieldRotation { get; set; } = new(Vector3.zero);

        public BeatValuePairList<SuperHexagonThemeFile> themes { get; set; } = new(new SuperHexagonThemeFile());

        public TypeList<SuperHexagonBarEffectFile> barEffect { get; set; } = new();

        public BeatValuePairAniListDouble globalNoteDistance { get; set; } = new(8);
        public BeatValuePairList<double> globalNoteSpeed { get; set; } = new(1);
    }

    public sealed class SuperHexagonBarEffectFile
    {
        public BeatValuePairAniListDouble noteDistance { get; set; } = new(1);

        public BeatValuePairList<NoteConfigFile> noteConfig { get; set; } = new(new NoteConfigFile());
    }

    public sealed class SuperHexagonThemeFile
    {
        public BeatValuePairAniListFloat fieldXPosition { get; set; } = new(0);
        public BeatValuePairAniListFloat fieldYPosition { get; set; } = new(0);
        public BeatValuePairAniListFloat fieldZPosition { get; set; } = new(0);

        public BeatValuePairAniListFloat fieldXRotation { get; set; } = new(0);
        public BeatValuePairAniListFloat fieldYRotation { get; set; } = new(0);
        public BeatValuePairAniListFloat fieldZRotation { get; set; } = new(0);

        public BeatValuePairAniListFloat fieldAutoZRotationSpeed { get; set; } = new(1);
        public BeatValuePairList<bool> fieldAutoZRotationDisable { get; set; } = new(false);

        public BeatValuePairAniListColor backgroundColor { get; set; } = new(new Color(0.125f, 0.125f, 0.125f));
        public BeatValuePairAniListColor backgroundColorAlt { get; set; } = new(new Color(0.25f, 0.25f, 0.25f));

        public BeatValuePairList<bool> backgroundColorAltReversal { get; set; } = new(false);

        public BeatValuePairAniListColor mainColor { get; set; } = new(Color.white);
        public BeatValuePairAniListColor mainColorAlt { get; set; } = new(new Color(0.875f, 0.875f, 0.875f));

        public BeatValuePairList<bool> mainColorAltReversal { get; set; } = new(false);

        public float transitionLength { get; set; } = 0;
        public EasingFunction.Ease transitionEasingFunction { get; set; } = EasingFunction.Ease.Linear;
    }
}
