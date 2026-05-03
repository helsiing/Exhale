using Exhale.Plugins.ServiceLocators;
using Exhale.Scripts.Data;
using Exhale.Scripts.Services;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Cards.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class PlacementAudioController : MonoBehaviour
    {
        [Header("Clips")]
        [SerializeField] private AudioClip tileArmedClip;
        [SerializeField] private AudioClip cardLaunchedClip;
        [SerializeField] private AudioClip cardLandedClip;
        [SerializeField] private AudioClip cancelledClip;

        [Header("Volume")]
        [SerializeField] [Range(0f, 1f)] private float tileArmedVolume    = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float cardLaunchedVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float cardLandedVolume   = 1f;
        [SerializeField] [Range(0f, 1f)] private float cancelledVolume    = 0.5f;

        private AudioSource audioSource;
        private readonly ServiceReference<IPlacementService> placementService = new();

        private void Awake() => audioSource = GetComponent<AudioSource>();

        private void Start()
        {
            var service = placementService.Reference;
            if (service == null) return;
            service.OnTileArmed         += OnTileArmed;
            service.OnTileDisarmed      += OnTileDisarmed;
            service.OnCardLaunchStarted += OnCardLaunchStarted;
            service.OnCardLanded        += OnCardLanded;
        }

        private void OnDestroy()
        {
            var service = placementService.Reference;
            if (service == null) return;
            service.OnTileArmed         -= OnTileArmed;
            service.OnTileDisarmed      -= OnTileDisarmed;
            service.OnCardLaunchStarted -= OnCardLaunchStarted;
            service.OnCardLanded        -= OnCardLanded;
        }

        private void OnTileArmed(int2 _, Entity __)             => Play(tileArmedClip,    tileArmedVolume);
        private void OnTileDisarmed()                           => Play(cancelledClip,     cancelledVolume);
        private void OnCardLaunchStarted(HexPieceTemplate _, int2 __) => Play(cardLaunchedClip, cardLaunchedVolume);
        private void OnCardLanded(HexPieceTemplate _, int2 __)        => Play(cardLandedClip,   cardLandedVolume);

        private void Play(AudioClip clip, float volume)
        {
            if (clip != null)
                audioSource.PlayOneShot(clip, volume);
        }
    }
}
