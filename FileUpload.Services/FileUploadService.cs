using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FileUpload.Data.Extensions;
using FileUpload.Models.Domain;
using FileUpload.Models.Request;
using FileUpload.Services.Interfaces;

namespace FileUpload.Services
{
    public class FileUploadService : BaseService, IFileUploadService
    {
        public List<UploadedFile> GetAll()
        {
            List<UploadedFile> result = new List<UploadedFile>();
            this.DataProvider.ExecuteCmd(
                "User_Files_GetAll",
                inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    UploadedFile model = Mapper(reader);
                    result.Add(model);
                }
            );
            return result;
        }

        public UploadedFile GetById(int id)
        {
            UploadedFile model = null;
            this.DataProvider.ExecuteCmd(
                "User_Files_GetById",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", id);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    model = Mapper(reader);
                }

            );

            return model;
        }

        public void Update(FileUploadUpdate model)
        {
            this.DataProvider.ExecuteNonQuery(
                "User_Files_Update",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", model.Id);
                    paramCol.AddWithValue("@Description", model.Description);
                }
            );
        }

        public async Task<int> Insert(FileUploadAddRequest model)
        {

            int id = 0;
            this.DataProvider.ExecuteNonQuery(
                "User_Files_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = "@Id";
                    parm.SqlDbType = System.Data.SqlDbType.Int;
                    parm.Direction = System.Data.ParameterDirection.Output;
                    paramCol.Add(parm);

                    paramCol.AddWithValue("@FileName", model.FileName);
                    paramCol.AddWithValue("@Size", model.Size);
                    paramCol.AddWithValue("@Type", model.Type);
                    paramCol.AddWithValue("@SystemFileName", model.SystemFileName);
                    paramCol.AddWithValue("@ModifiedBy", model.ModifiedBy);
                },
                returnParameters: delegate (SqlParameterCollection paramCol)
                {
                    id = (int)paramCol["@Id"].Value;
                }
            );
            return id;
        }

        public UploadedFile Delete(int fileId)
        {
            UploadedFile uploadedFile = new UploadedFile();
            this.DataProvider.ExecuteCmd(
                "User_Files_Delete",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@FileId", fileId);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    uploadedFile = Mapper(reader);
                }
            );

            return uploadedFile;
        }

        UploadedFile Mapper(IDataReader reader)
        {
            UploadedFile file = new UploadedFile();
            int index = 0;
            file.Id = reader.GetSafeInt32(index++);
            file.FileName = reader.GetSafeString(index++);
            file.Size = reader.GetSafeInt32(index++);
            file.Type = reader.GetSafeString(index++);
            file.SystemFileName = reader.GetSafeString(index++);
            file.Description = reader.GetSafeString(index++);
            file.CreatedDate = reader.GetSafeDateTime(index++);
            file.ModifiedDate = reader.GetSafeDateTime(index++);
            file.Modifiedby = reader.GetSafeString(index++);
            return file;
        }
    }
}
