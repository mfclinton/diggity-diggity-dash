using System.Collections.Generic;
using Game.Input.Data;
using Game.Input.Data.Interfaces;
using Game.Actions.Base;
using Game.Actions.Digging.Stencils;
using Game.Actions.Digging.Stencils.Interfaces;
using Game.Terrain;
using Game.Terrain.Generation;
using UnityEngine;

namespace Game.Actions.Digging
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DiggingController : AController
    {        
        [Header("Digging Settings")]
        [SerializeField] private float digStrength = 0.1f;
        
        [SerializeField] private float digCooldown = 0.1f;
        [SerializeField] private float digMovementPenalty = 0.5f;
        
        [Header("Visual")]
        [SerializeField] private bool showDigPreview = true;
        
        [Header("Stencil Settings")]
        [SerializeReference] private IDiggingStencil diggingStencil;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource digSoundSource;

        // References
        private TerrainGrid terrainGrid;

        // Component References
        private Rigidbody2D rb;

        // Internal State
        private float digTimer;
        private bool isDigging;
        private Vector2 digDirection;
        private float originalDamping; // TODO: Fix

        private float nextDigSoundTime = 0f;

        // Getters
        public bool IsDigging => isDigging;

        private void Awake()
        {
            base.Awake();

            TerrainGenerator terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
            terrainGrid = terrainGenerator.TerrainGrid;

            rb = GetComponent<Rigidbody2D>();
            originalDamping = rb.linearDamping;
        }

        private void Update()
        {
            HandleInput();
            
            if (isDigging)
            {
                UpdateDigging();
            }
        }
        
        private void OnDisable()
        {
            if (isDigging)
                rb.linearDamping = originalDamping;
        }

        private void HandleInput()
        {
            if (inputProvider == null)
                return;

            // Input
            InputContext input = inputProvider.GetCurrentInput();
            
            // Update Dig Direction
            digDirection = Vector2.zero;
            if (input.Movement.magnitude > 0.01f)
                digDirection = input.Movement.normalized;
            
            // Enter Digging Mode
            if (input.IsDigButtonHeld)
            {
                StartDigging();
            }
            else if (input.IsDigButtonReleased)
            {
                StopDigging();
            }
        }

        private void UpdateDigging()
        {
            digTimer -= Time.deltaTime;
            if (digTimer <= 0)
            {
                PerformDig();
                digTimer = digCooldown;
            }
        }

        private void StartDigging()
        {
            if (isDigging)
                return;
            
            digTimer = 0f;
            isDigging = true;
            rb.linearDamping = originalDamping * (1f + digMovementPenalty);
        }

        private void StopDigging()
        {
            if (!isDigging)
                return;
            
            isDigging = false;
            rb.linearDamping = originalDamping;
        }

        private void PerformDig()
        {
            if (terrainGrid == null || diggingStencil == null)
                return;
            
            List<DigCell> affectedCells = diggingStencil.GetAffectedCells(terrainGrid, transform.position, digDirection);
            
            UpdateCellDensities(affectedCells);

            // Play Dig Sound
            if (digSoundSource != null && Time.time >= nextDigSoundTime)
            {
                digSoundSource?.Play();
                nextDigSoundTime = Time.time + digSoundSource.clip.length;
            }
        }
        
        private void UpdateCellDensities(List<DigCell> affectedCells)
        {
            foreach (DigCell digCell in affectedCells)
            {
                float effectiveStrength = digStrength * digCell.Strength;
                
                // Update Density
                float currentDensity = terrainGrid.GetDensity(digCell.Position.x, digCell.Position.y);
                float newDensity = Mathf.Max(0f, currentDensity - effectiveStrength);
                
                terrainGrid.SetDensity(digCell.Position.x, digCell.Position.y, newDensity);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showDigPreview || !Application.isPlaying || diggingStencil == null) 
                return;
                
            diggingStencil.DrawPreview(transform.position, digDirection);
        }
    }
}