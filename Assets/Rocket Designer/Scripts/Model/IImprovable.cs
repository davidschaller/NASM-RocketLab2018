
	interface IImprovable
	{
        float GetReliability();

        float GetImprovement();

        bool RollTheDice();

        void Improve(float improveValue);

        void SetLastImprovement(float value);
    }
