using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class EfectoFadeInOut : ISampleProvider
    {
        private ISampleProvider fuente;
        private int muestrasLeidas = 0;
        private float segundosTranscurridos = 0;
        private float duracionFadeIn;
        private float duracionFadeOut;
        private float inicioFadeOut;

        public EfectoFadeInOut(ISampleProvider fuente,
            float duracionFadeIn, float duracionFadeOut, float inicioFadeOut)
        {
            this.fuente = fuente;
            this.duracionFadeIn = duracionFadeIn;
            this.duracionFadeOut = duracionFadeOut;
            this.inicioFadeOut = inicioFadeOut;
        }

        public WaveFormat WaveFormat {
            get {
                return fuente.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);


            muestrasLeidas += read;
            segundosTranscurridos =
                (float)muestrasLeidas /
                (float)fuente.WaveFormat.SampleRate /
                (float)fuente.WaveFormat.Channels;

            if (segundosTranscurridos <= duracionFadeIn)
            {
                //Aplicar el efecto
                float factorEscala =
                    segundosTranscurridos /
                        duracionFadeIn;
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }

            if (segundosTranscurridos >= inicioFadeOut &&
                segundosTranscurridos <= inicioFadeOut + duracionFadeOut)
            {
                //Aplicar el efecto
                float factorEscala =
                    1 - ((segundosTranscurridos - inicioFadeOut) /
                        duracionFadeOut);
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *=
                        factorEscala;
                }
            }
            else if (segundosTranscurridos >= inicioFadeOut + duracionFadeOut)
            {
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] = 0.0f;
                }
            }

            return read;
        }
    }
}
