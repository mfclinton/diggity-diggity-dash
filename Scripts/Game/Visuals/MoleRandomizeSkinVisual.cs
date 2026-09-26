using UnityEngine;
using System.Collections.Generic;

namespace Game.Visuals
{
    [System.Serializable]
    public class MoleSkin
    {
        public string skinName;
        public Sprite bodySprite;
        public Sprite tailSprite;
        public Sprite fronLegSprite;
        public Sprite backLegSprite;
    }

    public class MoleRandomizeSkinVisual : MonoBehaviour
    {
        [Header("Skin")]
        [SerializeField] private int currentSkin;

        [Header("Sprite Renderers")]
        [SerializeField] private SpriteRenderer indicatorRenderer;
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer frontLegRenderer;
        [SerializeField] private SpriteRenderer backLegRenderer;
        [SerializeField] private SpriteRenderer tailRenderer;

        [Header("Available Skins")]
        [SerializeField] private MoleSkin[] availableSkins;

        private void Awake()
        {
            SetSkin(currentSkin);
        }

        public void RandomizeSkin()
        {
            MoleSkin selectedSkin = availableSkins[Random.Range(0, availableSkins.Length)];
            ApplySkin(selectedSkin);
        }

        private void ApplySkin(MoleSkin skin)
        {
            indicatorRenderer.color = Random.ColorHSV();
            if (bodyRenderer != null) bodyRenderer.sprite = skin.bodySprite;
            if (frontLegRenderer != null) frontLegRenderer.sprite = skin.fronLegSprite;
            if (backLegRenderer != null) backLegRenderer.sprite = skin.backLegSprite;
            if (tailRenderer != null) tailRenderer.sprite = skin.tailSprite;
        }

        public void SetSkin(int skinIndex)
        {
            ApplySkin(availableSkins[skinIndex]);
        }
    }
}