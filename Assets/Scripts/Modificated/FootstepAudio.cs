using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private FootstepRaycast _raycast;
    [SerializeField] List<MaterialAudio> _materials;
    [SerializeField] AudioSource _audioSource;
    Dictionary <MaterialType, AudioClip> _materialAudioMap;
    private MaterialType _currentMaterial;

    private void Awake()
    {
        _materialAudioMap = new Dictionary<MaterialType, AudioClip>();

        foreach(var m in _materials)
        {
            _materialAudioMap[m.material] = m.clip;
        }
    }

    private void OnEnable()
    {
        _raycast.OnSurfaceHit += SetMaterial;
    }

    private void OnDisable()
    {
        _raycast.OnSurfaceHit -= SetMaterial;
    }

    private void SetMaterial(MaterialType material)
    {
        _currentMaterial = material;
    }

    public void PlayStep()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.2f);

        if (_materialAudioMap.TryGetValue(_currentMaterial, out var clip) && clip != null)
        {
            _audioSource.PlayOneShot(_materialAudioMap[_currentMaterial]);
        }
    }
}