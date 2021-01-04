using image_cloud_processor.MLModels;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Service
{

    public class PredictionMLService
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
        public PredictionMLService(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public ModelOutput Predict(ModelInput input)
        {
            return Predict(input, "Field_SexoModel");
        }
        public ModelOutput Predict(ModelInput input, string modelName)
        {

            ModelOutput prediction = _predictionEnginePool.GetPredictionEngine(modelName).Predict(example: input);

            return prediction;
        }

    }
}
