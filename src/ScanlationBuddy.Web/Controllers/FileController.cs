namespace ScanlationBuddy.Web.Controllers;

[ApiController]
public class FileController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IFileUploadService _upload;

	public FileController(IDbService db, IFileUploadService upload)
	{
		_db = db;
		_upload = upload;
	}

}
