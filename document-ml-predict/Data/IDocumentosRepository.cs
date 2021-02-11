using image_cloud_processor.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace image_cloud_processor.Repository
{
    public interface IDocumentosRepository<T>
    {
        T ObterDocumento(string codigo);
        T ObterDocumentoById(MongoDB.Bson.ObjectId id);

        T SalvarOuAtualizarDocumento(T documento);
        IEnumerable<T> ListarDocumentos();

        MongoDB.Bson.ObjectId AttachFile(byte[] source);
        byte[] DownloadFile(MongoDB.Bson.ObjectId id);
        IEnumerable<T> ListarDocumentos(StatusDocumento status);

        Task<Document> AtualizarDocumentoAsync(Document documento);

    }
}