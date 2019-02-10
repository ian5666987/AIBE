using System;
using System.Collections.Generic;

namespace Aibe.Models {
  public class NavDataModel {
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int QueryCount { get; set; }
    public int MaxPage { get; set; }
    public int PrevPage { get; set; }
    public int Prev10Page { get; set; }
    public int Prev100Page { get; set; }
    public int NextPage { get; set; }
    public int Next10Page { get; set; }
    public int Next100Page { get; set; }
    public int ItemNoInPageFirst { get; set; }
    public int ItemNoInPageLast { get; set; }
    public string FirstKey { get; set; }
    public string LastKey { get; set; }
    public bool KeysUsed { get; set; }
    public bool DataNameUsed { get; set; }
    public string DataName { get; set; }
    public string ParentPage { get; set; }
    public string PrevPageMessage { get; private set; }
    public string Prev10PagesMessage { get; private set; }
    public string Prev100PagesMessage { get; private set; }
    public string FirstPageMessage { get; private set; }
    public string NextPageMessage { get; private set; }
    public string Next10PagesMessage { get; private set; }
    public string Next100PagesMessage { get; private set; }
    public string LastPageMessage { get; private set; }
    public NavDataModel(int currentPage, int itemsPerPage, int queryCount) {
      PrevPageMessage = LCZ.W_PreviousPage;
      Prev10PagesMessage = LCZ.W_Previous10Pages;
      Prev100PagesMessage = LCZ.W_Previous100Pages;
      FirstPageMessage = LCZ.W_FirstPage;
      NextPageMessage = LCZ.W_NextPage;
      Next10PagesMessage = LCZ.W_Next10Pages;
      Next100PagesMessage = LCZ.W_Next100Pages;
      LastPageMessage = LCZ.W_LastPage;
      UpdateModel(currentPage, itemsPerPage, queryCount);
    }

    public void UpdateModel(int currentPage, int itemsPerPage, int queryCount) {
      CurrentPage = currentPage;
      ItemsPerPage = itemsPerPage;
      QueryCount = queryCount;
      if (queryCount > 0) {
        MaxPage = (int)Math.Ceiling((decimal)QueryCount / ItemsPerPage);
        CurrentPage = Math.Max(1, Math.Min(MaxPage, CurrentPage));
        PrevPage = (int)Math.Max(1m, CurrentPage - 1);
        Prev10Page = (int)Math.Max(1m, CurrentPage - 10);
        Prev100Page = (int)Math.Max(1m, CurrentPage - 100);
        NextPage = (int)Math.Min((decimal)MaxPage, CurrentPage + 1);
        Next10Page = (int)Math.Min((decimal)MaxPage, CurrentPage + 10);
        Next100Page = (int)Math.Min((decimal)MaxPage, CurrentPage + 100);
        ItemNoInPageFirst = (CurrentPage - 1) * ItemsPerPage + 1;
        ItemNoInPageLast = Math.Min(CurrentPage * ItemsPerPage, QueryCount);
      } else {
        MaxPage = 1;
        CurrentPage = 1;
        PrevPage = 1;
        Prev10Page = 1;
        Prev100Page = 1;
        NextPage = 1;
        Next10Page = 1;
        Next100Page = 1;
        ItemNoInPageFirst = 0;
        ItemNoInPageLast = 0;
      }
    }

    public void GoToFirstPage() { UpdateModel(1, ItemsPerPage, QueryCount); }
    public void GoToPrevPage() { UpdateModel(PrevPage, ItemsPerPage, QueryCount); }
    public void GoToPrev10Page() { UpdateModel(Prev10Page, ItemsPerPage, QueryCount); }
    public void GoToPrev100Page() { UpdateModel(Prev100Page, ItemsPerPage, QueryCount); }
    public void GoToNextPage() { UpdateModel(NextPage, ItemsPerPage, QueryCount); }
    public void GoToNext10Page() { UpdateModel(Next10Page, ItemsPerPage, QueryCount); }
    public void GoToNext100Page() { UpdateModel(Next100Page, ItemsPerPage, QueryCount); }
    public void GoToLastPage() { UpdateModel(MaxPage, ItemsPerPage, QueryCount); }
  }
}