# GameDemo
Strategy Game Demo
Yapan : Alperen Keser

Grid Görünümü :
  Grid görünümü için Game penceresinde gizmos açık olduğunda görünür hale gelir.
  Her bir gridin içinde konumu ve eğer obje yer alıyorsa ismi text mesh üzerinden ismi yazılır. 
  Bu görünümleri kapatmak için Grid.cs c# dosyasında debug seçeneği true'dan false çekilirse görünüm kapanır.
  
MainGameObject içinde SelectingObjectController ile Grid oluşturma, arayüzden grid'e yerleştirilecek objeleri seçme ve yerleştirme,
yerleştirilen objelerin detaylarını gösterme ve asker oluşturma kımları bulunmaktadır. Sol mouse tuşu seçim, sağ mouse tuşu boş bir yere
tıklanırsa seçim iptali eğer asker seçilmişse pathfinding algoritması çalışır.

Information arayüzüne seçilen objenin resmi ismi ve eğer asker çıkatabiliyorsa asker basma seçeneklerini InformationPanel.cs dosyasında oluşturdum.

Seçilen askerin ilerlemesi için soldier prefab'in içine SoldierMovement.cs dosyasını oluşturdum. Bu dosyada seçilen askerin gitmesi gereken konumu alarak
Pathfinding.cs dosyası ile A* pathfinding algorithması çalıştırmaktayım. Eğer hareket halinde gidiş yönünde bir obje oluşturulursa tekrardan yolu bulmaya çalışır.

Ekstra olarak kamera hareketleri ekledim. WASD, oktuşları veya sağ mouse tuşuna basılarak hareket ettirilebilir ve mouse ortasıyla yaklaştırılıp uzaklaştırılabilir. 

Böyle bir oyun projesiyle verdiğiniz deneğim için şimdiden teşekkürler, iyi denemeler.
