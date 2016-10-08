<%@ Control Language="C#"%>
<%@ Import Namespace="System.Drawing" %>
<script runat="server">

    public string ImagePath
    {
        get { return Request["imgPath"]; }
    }
    
    public string id = Guid.NewGuid().ToString();

    private Size size;
    public Size Size
    {
        get { return size != default(Size) ? size : (size = GetImageFromDZPxCell.GetSize(Request["imgPath"])); }
        set { size = value; }
    }

</script>


<div id='openseadragon<%:this.id%>' style='width: 800px; height: 600px;'></div>
 <script type='text/javascript'>
        var viewer = OpenSeadragon({
            id: 'openseadragon<%:this.id%>',
            prefixUrl: '/openseadragon/images/',
            tileSources: {
                Image: {
                    xmlns: 'http://schemas.microsoft.com/deepzoom/2008',
                    Url: 'dzi/img_files/',
                    Format: 'jpg',
                    Overlap: '2',
                    TileSize: '256',
                    Size: {
                        Height: <%:this.Size.Height%>,
                        Width: <%:this.Size.Width%>,
                    }
                }
            }
        });
      </script>