using UnityEngine;

public enum PlantStatus {seed, phase1, phase2, phase3, has_growed, phase1_dry, phase2_dry, phase3_dry}

public interface IPlant 
{
    int GrowingPhase { get; set; }
    Sprite PhaseSprite { get; set; }
    GrowableObject growableObject { get; }
    PlantStatus plantStatus { get; set; }

    void ToNextPhase();
    void Grow();
}

