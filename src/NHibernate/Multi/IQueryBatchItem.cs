using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Multi
{
	/// <summary>
	/// Interface for wrapping query to be batched by <see cref="IQueryBatch"/>.
	/// </summary>
	public interface IQueryBatchItem<TResult> : IQueryBatchItem
	{
		/// <summary>
		/// Return loaded typed results by query.
		/// Must be called only after <see cref="IQueryBatch.Execute"/>.
		/// </summary>
		IList<TResult> GetResults();

		/// <summary>
		/// A callback, executed after results are loaded by the batch.
		/// Loaded results are provided as the action parameter.
		/// </summary>
		Action<IList<TResult>> AfterLoadCallback { get; set; }
	}

	/// <summary>
	/// Interface for wrapping query to be batched by <see cref="IQueryBatch"/>.
	/// </summary>
	public partial interface IQueryBatchItem
	{
		/// <summary>
		/// Optionally, the query caching information list, for batching. Each element matches
		/// a SQL-Query resulting from the query translation, in the order they are translated.
		/// It should yield an empty enumerable if no batching of caching is handled for this
		/// query.
		/// </summary>
		IEnumerable<ICachingInformation> CachingInformation { get; }

		/// <summary>
		/// Initialize the query. Method is called right before batch execution.
		/// Can be used for various delayed initialization logic.
		/// </summary>
		/// <param name="session"></param>
		void Init(ISessionImplementor session);

		/// <summary>
		/// Get the query spaces.
		/// </summary>
		/// <remarks>
		/// Query spaces indicates which entity classes are used by the query and need to be flushed
		/// when auto-flush is enabled. It also indicates which cache update timestamps needs to be
		/// checked for up-to-date-ness.
		/// </remarks>
		IEnumerable<string> GetQuerySpaces();
		
		/// <summary>
		/// Get the commands to execute for getting the not-already cached results of this query.
		/// </summary>
		/// <returns>The commands for obtaining the results not already cached.</returns>
		IEnumerable<ISqlCommand> GetCommands();

		/// <summary>
		/// Process the result sets generated by <see cref="GetCommands"/>. Advance the results set
		/// to the next query, or to its end if this is the last query.
		/// </summary>
		/// <returns>The number of rows processed.</returns>
		int ProcessResultsSet(DbDataReader reader);

		/// <summary>
		/// Process the results of the query, including cached results.
		/// </summary>
		/// <remarks>Any result from the database must have been previously processed
		/// through <see cref="ProcessResultsSet"/>.</remarks>
		void ProcessResults();

		/// <summary>
		/// Execute immediately the query as a single standalone query. Used in case the data-provider
		/// does not support batches.
		/// </summary>
		void ExecuteNonBatched();
	}
}
