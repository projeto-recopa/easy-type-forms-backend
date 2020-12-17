using image_cloud_processor.Models;
using System.Collections.Generic;
using System.IO;

namespace image_cloud_processor.Repository
{
    public interface IDocumentosRepository
    {
        T ObterDocumento<T>(string codigo);

        T SalvarOuAtualizarDocumento<T>(T documento);
        IEnumerable<T> ListarDocumentos<T>();

        MongoDB.Bson.ObjectId AttachFile(byte[] source);
        byte[] DownloadFile(MongoDB.Bson.ObjectId id);
    }
}