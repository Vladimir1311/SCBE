using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Storage.Service.Utilites
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!operation.OperationId.EndsWith("ApiV1StorageLoadFileByOwnerByPathPost"))
                return;

            operation.Parameters = operation.Parameters.Where(p => p.In != "query").ToList();
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "uploadedFile",
                In = "formData",
                Description = "Upload File",
                Required = true,
                Type = "file"
            });
            operation.Consumes.Add("multipart/form-data");
        }
    }
}
