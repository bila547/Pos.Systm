
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sps.Pos.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sps.Pos.Api
{
	[ApiController]
	public class ApiBaseController : ControllerBase
	{
		protected readonly PosDbContext _context;

		public ApiBaseController(PosDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		protected List<string> GetErrorListFromModelState(ModelStateDictionary modelState)
		{
			var query = from state in modelState.Values
						from error in state.Errors
						select error.ErrorMessage;

			return query.ToList();
		}
	}
}