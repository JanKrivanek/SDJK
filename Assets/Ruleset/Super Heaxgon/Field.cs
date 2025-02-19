using SCKRM;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SuperHexagon.Judgement;
using System.Threading;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class Field : MonoBehaviour
    {
        [SerializeField] SuperHexagonManager _manager; public SuperHexagonManager manager => _manager;
        [SerializeField] SuperHexagonJudgementManager _judgementManager; public SuperHexagonJudgementManager judgementManager => _judgementManager;
        public SuperHexagonMapFile map => manager.map;

        [SerializeField] Walls _walls; public Walls walls => _walls;
        [SerializeField] float _audioSize = 1; public float audioSize { get => _audioSize; set => _audioSize = value; }
        [SerializeField] float _audioSizeLerp = 0.5f; public float audioSizeLerp { get => _audioSizeLerp; set => _audioSizeLerp = value; }

        public double zoom { get; private set; }
        public double sides { get; private set; } = 6;

        public float globalWallOffset { get; set; } = 0;

        public Color backgroundColor { get; set; } = Color.clear;
        public Color backgroundColorAlt { get; set; } = Color.clear;

        public bool isBackgroundAltReversal { get; set; } = false;

        public Color mainColor { get; set; } = Color.clear;
        public Color mainColorAlt { get; set; } = Color.clear;

        public bool isMainColorAltReversal { get; set; } = false;

        public void Refresh() => walls.Refresh();

        bool onAudioFilterReadEventEnable = false;
        void Update()
        {
            if (!RhythmManager.isPlaying || map == null)
                return;

            zoom = zoom.Lerp(audioZoom, audioSizeLerp * Kernel.fpsUnscaledSmoothDeltaTime);

            double realSides = map.effect.sidesList.GetValue(RhythmManager.currentBeatScreen);
            if (realSides < sides)
                sides = sides.MoveTowards(realSides, (sides.Ceil() - realSides) * (0.125f * Kernel.fpsSmoothDeltaTime));
            else
                sides = sides.MoveTowards(realSides, (realSides - sides.Floor()) * (0.125f * Kernel.fpsSmoothDeltaTime));

            if (manager.soundPlayer != null && !onAudioFilterReadEventEnable)
            {
                manager.soundPlayer.onAudioFilterReadEvent += AudioZoomUpdate;
                onAudioFilterReadEventEnable = true;
            }
        }

        float audioZoom
        {
            get
            {
                while (Interlocked.CompareExchange(ref audioZoomLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float value = _audioZoom;
                
                Interlocked.Decrement(ref audioZoomLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref audioZoomLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _audioZoom = value;
                Interlocked.Decrement(ref audioZoomLock);
            }
        }
        float _audioZoom = 1;
        int audioZoomLock = 0;

        void OnDisable()
        {
            if (manager.soundPlayer != null)
                manager.soundPlayer.onAudioFilterReadEvent -= AudioZoomUpdate;

            onAudioFilterReadEventEnable = false;
        }

        void AudioZoomUpdate(ref float[] data, int channels)
        {
            float finalSample = 0;
            for (int i = 0; i < channels; i++)
            {
                float sampleChannel = 0;
                for (int j = i; j < data.Length; j += channels)
                {
                    float sample = data[j].Abs();
                    sampleChannel += sample;
                }

                finalSample += sampleChannel / channels;
            }

            audioZoom = 0.8f + (finalSample * audioSize);
        }
    }
}
