using System.Collections;

public class BleedPresets {

	//TODO: USE UNITY PERSISTENCE

	public class PresetBase {
		public int bleedScanRadius = 2;
		public int bleedThresholdPerc = 80;
		public int bleedIterations = 1;
		public int bleedChancePercent = 75;
	}

	public class SmoothEdges : PresetBase {
//		bleedScanRadius = 3;
//		bleedThresholdPerc = 75;
//		bleedIterations = 1;
//		bleedChancePercent = 80;
	}

	public enum SmoothEdgesBig {
		bleedScanRadius = 3,
		bleedThresholdPerc = 75,
		bleedIterations = 1,
		bleedChancePercent = 80
	}

	public enum SmoothAllEdges {
		
	}

	public enum ExpandDenselyPopulatedTerrains {
		
	}

	public enum AddSomeRandom {
		
	}

	public enum AddTotalRandomness {
		
	}

}
