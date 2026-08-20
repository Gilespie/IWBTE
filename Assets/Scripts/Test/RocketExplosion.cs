using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RocketExplosion : MonoBehaviour
{
    public static Action OnExplosed;
    [SerializeField] float _delay;
    [SerializeField] VisualEffect _vfx;
    [SerializeField] AudioClip _clip;
    AudioSource _source;

    private void Awake()
    {
        _source = GetComponentInChildren<AudioSource>();
        _vfx.Stop();
    }

    public void ActivateVFX()
    {
        _vfx.Play();
        Invoke(nameof(DelaySound), _delay);
    }

    private void DelaySound()
    {
        _source.PlayOneShot(_clip);
        OnExplosed?.Invoke();
    }
}
