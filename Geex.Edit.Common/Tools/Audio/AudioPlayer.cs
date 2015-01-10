using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
namespace Geex.Edit.Common.Tools.Audio
{
    /// <summary>
    /// Wrapper for playing audio.
    /// </summary>
    public class AudioPlayer
    {
        /// <summary>
        /// Starts the player service
        /// </summary>
        public AudioPlayer()
        {
            XnaControl.XnaFrameworkDispatcherService.StartService();
        }
        /// <summary>
        /// Plays the song given its filename.
        /// </summary>
        /// <param name="filename"></param>
        public void PlaySong(string filename)
        {
            Song song = Song.FromUri("name", new Uri(filename, UriKind.Relative));
            MediaPlayer.Play(song);
            
        }
        /// <summary>
        /// Changes the volume of the song
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeVolume(int volume)
        {
            MediaPlayer.Volume = volume / 100.0f;
        }
        /// <summary>
        /// Changes the pitch of the song
        /// </summary>
        /// <param name="pitch"></param>
        public void ChangePitch(int pitch)
        {

        }
        /// <summary>
        /// Stops the song.
        /// </summary>
        public void StopSong()
        {
            MediaPlayer.Stop();
        }
        /// <summary>
        /// Ends the service.
        /// </summary>
        public void End()
        {
            MediaPlayer.Stop();
            XnaControl.XnaFrameworkDispatcherService.StopService();
        }
    }

    #region Obsolete
    /*/// <summary>
    /// Wrapper for playing audio (old)
    /// </summary>
    class AudioPlayerQuartz
    {
        QuartzTypeLib.FilgraphManager m_graphManager;
        QuartzTypeLib.IMediaControl m_mediaControl;
        public AudioPlayerQuartz()
        {
            m_graphManager = new QuartzTypeLib.FilgraphManager();
            m_mediaControl = (QuartzTypeLib.IMediaControl)m_graphManager;
        }
        public void PlaySong(string filename)
        {
            StopSong();
            m_mediaControl.RenderFile(filename);
            m_mediaControl.Run();
        }
        public void ChangeVolume(int volume)
        {
            ((QuartzTypeLib.IBasicAudio)m_graphManager).Volume = volume;
        }
        public void ChangePitch(int pitch)
        {

        }
        public void StopSong()
        {
            m_mediaControl.Stop();
        }
        public void End()
        {
        }
    }*/

    #endregion
}
