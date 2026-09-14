using UnityEngine;

namespace LumenRush
{
    public sealed class AudioManager : MonoBehaviour
    {
        GameManager game;
        AudioSource effects, music;
        AudioClip[] tones;
        float step;
        public void Initialize(GameManager game)
        {
            this.game = game;
            effects = gameObject.AddComponent<AudioSource>();
            music = gameObject.AddComponent<AudioSource>();
            tones = new AudioClip[8];
            float[] notes = {65, 130, 220, 330, 420, 660, 880, 1050};
            for (int i = 0; i < notes.Length; i++)
                tones[i] = Tone(notes[i], .22f);
            int sampleRate = 22050;
            float[] data = new float[sampleRate * 8];
            int[] melody = {0, 3, 5, 7, 5, 3, 2, 0, 0, 2, 5, 3, 7, 5, 3, 2};
            float[] scale = {220, 247, 261.63f, 293.66f, 329.63f, 349.23f, 392, 440};
            for (int i = 0; i < data.Length; i++)
            {
                float t = (float)i / sampleRate;
                float beat = t % .5f;
                float n = scale[melody[(int)(t * 2) % 16]];
                data[i] = Mathf.Sin(t * n * Mathf.PI * 2) * Mathf.Exp(-beat * 9) * .09f + Mathf.Sin(t * 55 * Mathf.PI * 2) * .035f;
            }

            music.clip = AudioClip.Create("Original - Afterlight circuit", data.Length, 1, sampleRate, false);
            music.clip.SetData(data, 0);
            music.loop = true;
            music.Play();
        }

        AudioClip Tone(float hz, float seconds)
        {
            int rate = 22050;
            float[] data = new float[(int)(rate * seconds)];
            for (int i = 0; i < data.Length; i++)
                data[i] = Mathf.Sin(i * hz * 2 * Mathf.PI / rate) * .14f * (1 - (float)i / data.Length);
            var clip = AudioClip.Create("Synth " + hz, data.Length, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public void PlayTone(float hz, float duration)
        {
            if (!game.Save.Data.sound)
                return;
            int best = 0;
            float[] notes = {65, 130, 220, 330, 420, 660, 880, 1050};
            for (int i = 1; i < notes.Length; i++)
                if (Mathf.Abs(notes[i] - hz) < Mathf.Abs(notes[best] - hz))
                    best = i;
            effects.PlayOneShot(tones[best]);
        }

        public void ApplySettings()
        {
            music.mute = !game.Save.Data.music;
            effects.mute = !game.Save.Data.sound;
        }

        void Update()
        {
            if (game.State != RunState.Running)
                return;
            step += Time.deltaTime;
            if (step > .32f && game.Player.Height < .1f)
            {
                step = 0;
                effects.volume = .6f;
                PlayTone(220, .04f);
            }
        }

        void OnDestroy()
        {
            if (tones != null)
                foreach (var clip in tones)
                    Destroy(clip);
            if (music != null)
                Destroy(music.clip);
        }
    }
}
