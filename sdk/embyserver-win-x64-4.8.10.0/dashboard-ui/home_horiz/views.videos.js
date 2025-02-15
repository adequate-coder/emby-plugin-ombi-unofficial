define(["exports","./../modules/emby-apiclient/connectionmanager.js","./../modules/cardbuilder/cardbuilder.js","./../modules/common/itemmanager/itemmanager.js","./../modules/emby-elements/emby-itemscontainer/emby-itemscontainer.js","./../modules/common/globalize.js","./../modules/approuter.js","./../modules/tabbedview/basetab.js"],function(_exports,_connectionmanager,_cardbuilder,_itemmanager,_embyItemscontainer,_globalize,_approuter,_basetab){function VideosTab(view,params){_basetab.default.apply(this,arguments)}Object.defineProperty(_exports,"__esModule",{value:!0}),_exports.default=void 0,Object.assign(VideosTab.prototype,_basetab.default.prototype),VideosTab.prototype.scrollDirection=function(){return"x"},VideosTab.prototype.onTemplateLoaded=function(){_basetab.default.prototype.onTemplateLoaded.apply(this,arguments);var view=this.view;this.latestItemsContainer=view.querySelector(".latestSection .itemsContainer"),this.latestItemsContainer.fetchData=function(){var options={IncludeItemTypes:"Video",Limit:12,ParentId:this.params.parentId,EnableImageTypes:this.getRequestedImageTypes(),Fields:this.getRequestedItemFields()+",PrimaryImageAspectRatio",ImageTypeLimit:1,EnableTotalRecordCount:!1};return this.getApiClient().getLatestItems(options)}.bind(this),this.latestItemsContainer.getListOptions=function(){return{renderer:_cardbuilder.default,options:{shape:"auto",horizontalGrid:!0,scalable:!1,multiSelect:!1}}}.bind(this),this.latestItemsContainer.parentContainer=this.latestItemsContainer.closest(".horizontalSection"),this.addFocusBehavior(this.latestItemsContainer),this.resumeItemsContainer=view.querySelector(".resumeSection .itemsContainer"),this.resumeItemsContainer.fetchData=function(){var options={IncludeItemTypes:"Video",Limit:6,ParentId:this.params.parentId,ImageTypeLimit:1,EnableImageTypes:this.getRequestedImageTypes(),EnableTotalRecordCount:!1},apiClient=this.getApiClient();return apiClient.getResumableItems(apiClient.getCurrentUserId(),options)}.bind(this),this.resumeItemsContainer.getListOptions=function(){return{renderer:_cardbuilder.default,options:{shape:"backdrop",horizontalGrid:!0,preferThumb:!0,scalable:!1,multiSelect:!1}}}.bind(this),this.resumeItemsContainer.parentContainer=this.resumeItemsContainer.closest(".horizontalSection"),this.addFocusBehavior(this.resumeItemsContainer),this.categoryItemsContainer=view.querySelector(".categoryItemsContainer"),this.categoryItemsContainer.fetchData=function(){var apiClient=this.getApiClient(),parentId=this.params.parentId,items=[],subviews=(items.push({Name:_globalize.default.translate("Videos"),Id:"videos_videos",ServerId:apiClient.serverId(),ParentId:parentId,Icon:_itemmanager.default.getDefaultIcon({Type:"Video",MediaType:"Video"}),IsCategory:!0}),this.item.Subviews);return subviews.includes("photos")&&items.push({Name:_globalize.default.translate("Photos"),Id:"videos_photos",ServerId:apiClient.serverId(),ParentId:parentId,Icon:_itemmanager.default.getDefaultIcon({Type:"Photo",MediaType:"Photo"}),IsCategory:!0}),subviews.includes("artists")&&items.push({Name:_globalize.default.translate("Artists"),Id:"videos_artists",ServerId:apiClient.serverId(),ParentId:parentId,Icon:_itemmanager.default.getDefaultIcon({Type:"MusicArtist"}),IsCategory:!0}),items.push({Name:_globalize.default.translate("Folders"),Id:"videos_folders",ServerId:apiClient.serverId(),ParentId:parentId,Icon:_itemmanager.default.getDefaultIcon({Type:"Folder"}),IsCategory:!0}),Promise.resolve({Items:items,TotalRecordCount:items.length})}.bind(this),this.categoryItemsContainer.getListOptions=function(){return{renderer:_cardbuilder.default,options:{shape:"square",multiSelect:!1,contextMenu:!1,overlayText:!0,fields:["Name"],action:"custom",horizontalGrid:!0}}}.bind(this),this.categoryItemsContainer.addEventListener("action-null",function(e){var e=e.detail.item,url="/videos?serverId="+_connectionmanager.default.getApiClient(e).serverId()+"&parentId="+e.ParentId;1<(e=e.Id.split("_")).length&&(url+="&tab="+e[1]),_approuter.default.show(url)}.bind(this)),this.addFocusBehavior(this.categoryItemsContainer)},VideosTab.prototype.getItem=function(){var apiClient=this.getApiClient(),instance=this;return instance.item?Promise.resolve():apiClient.getItem(apiClient.getCurrentUserId(),this.params.parentId).then(function(item){return instance.view.querySelector(".latestHeader").innerHTML=_globalize.default.translate("LatestFromLibrary",item.Name),instance.item=item})},VideosTab.prototype.onResume=function(options){_basetab.default.prototype.onResume.apply(this,arguments);var instance=this;return this.getItem().then(function(){var promises=[instance.categoryItemsContainer.resume(options).then(function(){return options.autoFocus&&instance.autoFocus(),Promise.resolve()}),instance.resumeItemsContainer.resume(options),instance.latestItemsContainer.resume(options)];return Promise.all(promises)})},VideosTab.prototype.onPause=function(){_basetab.default.prototype.onPause.apply(this,arguments),this.latestItemsContainer.pause(),this.resumeItemsContainer.pause(),this.categoryItemsContainer.pause()},VideosTab.prototype.destroy=function(){_basetab.default.prototype.destroy.apply(this,arguments),this.latestItemsContainer=null,this.resumeItemsContainer=null,this.categoryItemsContainer=null},VideosTab.prototype.loadTemplate=function(){return require(["text!home_horiz/views.videos.html"])};_exports.default=VideosTab});