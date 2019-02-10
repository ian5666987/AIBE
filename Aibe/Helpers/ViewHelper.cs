using Aibe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Aibe.Helpers {
  public class ViewHelper {
    public static List<T> PrepareFilteredModels<T>(int? page, IOrderedQueryable<T> filtereds, out NavDataModel navDataModel) {
      List<T> filteredModels = null;
      int itemsPerPage;
      bool resultParse = int.TryParse(ConfigurationManager.AppSettings["itemsPerPage"], out itemsPerPage);
      if (!resultParse)
        itemsPerPage = 20; //default
      int queryCount = 0;
      if (filtereds.Any()) {
        ViewPerPageQueryResult<T> result = ViewHelper.ProcessView(page, filtereds, itemsPerPage);
        filteredModels = result.Results.ToList();
        queryCount = result.QueryCount;
        itemsPerPage = result.ItemsPerPage;
      }
      int pageValue = page.HasValue ? page.Value : 1;
      int maxPage = ((int)queryCount + itemsPerPage - 1) / itemsPerPage;
      int currentPage = Math.Max(Math.Min(pageValue, maxPage), 1);
      navDataModel = new NavDataModel(currentPage, itemsPerPage, queryCount);
      return filteredModels;
    }

    public static ViewPerPageQueryResult<TEntity> ProcessView<TEntity>(int? page, IOrderedQueryable<TEntity> allOrderedMatches, int itemsPerPage) {
      page = page == null ? 1 : page;
      int skippedItems = 0;
      int queryFound = allOrderedMatches.Any() ? allOrderedMatches.Count() : 0;
      decimal maxPage = Math.Ceiling((decimal)queryFound / itemsPerPage);
      page = (int)Math.Min(Math.Max(1, page.Value), maxPage);
      skippedItems = (page.Value - 1) * itemsPerPage;
      var matchResults = allOrderedMatches
        .Skip(skippedItems).Take(itemsPerPage); //take some each time
      ViewPerPageQueryResult<TEntity> result = new ViewPerPageQueryResult<TEntity>();
      result.Page = page;
      result.QueryCount = queryFound;
      result.ItemsPerPage = itemsPerPage;
      result.Results = matchResults;
      return result;
    }
  }

  public class ViewPerPageQueryResult<TEntity> {
    public int? Page { get; set; }
    public int QueryCount { get; set; }
    public int ItemsPerPage { get; set; }
    public IQueryable<TEntity> Results { get; set; }
  }
}