using System;

public interface MapArrayRepository {
	
	void PersistNoiseValues();
	void PersistGraphValues();
	void PersistFinalValues();
	void PersistRecentValues();

	void GetNoiseValues();
	void GetGraphValues();
	void GetFinalValues();
	void GetRecentValues();

}

