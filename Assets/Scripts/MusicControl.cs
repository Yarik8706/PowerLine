using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicControl : MonoBehaviour
{
    [SerializeField] private Image musicIconImage;
    [SerializeField] private Sprite disableMusicIcon;
    [SerializeField] private Sprite activeMusicIcon;
    [SerializeField] private AudioMixer audioMixer;
    
    // private bool _effectsMusicActive = true;
    public bool MusicActive { get; private set; } = true;

    public static MusicControl Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeMusicActive()
    {
        MusicActive = !MusicActive;
        YandexGameControl.Instance.SaveOtherData();
        musicIconImage.sprite = MusicActive ? activeMusicIcon : disableMusicIcon;
        audioMixer.SetFloat("Music", MusicActive ? 0 : -80);
    }
    
    // public void ChangeEffectsMusicActive()
    // {
    //     _effectsMusicActive = !_effectsMusicActive;
    //     audioMixer.SetFloat("Effects", _effectsMusicActive ? 0 : -80);
    // }
}