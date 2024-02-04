using System;
using System.Collections.Generic;
using System.Linq;  //liste işlemleri için var
using System.Threading; //Timer için bu sınıfı kullandım

class Kitap
{
    public string Baslik { get; set; }  //kitap başlığı
    public string Yazar { get; set; }   // kitabın yazarı
    public string ISBN { get; set; }    //ISBN numarası
    public int KopyaSayisi { get; set; }    //Toplam kitap kopya sayısı
    public int OduncAlinanKopyaSayisi { get; set; } // şu anda ödünç almış olduğumuz kitap kopya sayımız
    public int OduncSuresiDakika { get; set; } = 1; // kitabın ödünç alınma süresi, dakika cinsinden
    private Timer oduncSuresiTimer; //ödünç süresi takibinde kullandığımız sayaç

    public void OduncAlindi()
    {
        // ödünç alınan kitap sayısını güncellemek ve mesajı kullanıcıya yazdırmak için kullanıyoruz
        OduncAlinanKopyaSayisi = Math.Min(OduncAlinanKopyaSayisi + 1, KopyaSayisi);

        Console.WriteLine($"{Baslik} başlıklı kitap ödünç alındı. Kitabı {OduncSuresiDakika} dakika içinde iade ediniz.");

        // zamanlayıcı başlatıyoruz ve bittiğinde çağrılacak metodu belirliyoruz.
        oduncSuresiTimer = new Timer(OduncSuresiBitti, null, OduncSuresiDakika * 60 * 1000, Timeout.Infinite);
    }

    private void OduncSuresiBitti(object state)
    {
        //yukarda çağrılıyor bu metod, bittiğinde süre alttaki mesaj yazdırılıyor.
        OduncSuresiGecmis = true;
        Console.WriteLine($"{Baslik} başlıklı kitabın süresi geçti, iade etmeniz gerek.");
    }

    public bool OduncSuresiGecmis { get; private set; }
}

class Kutuphane
{
    private List<Kitap> kitaplar = new List<Kitap>();   //kitaplarımızın tutulduğu listemiz

    public void KitapEkle(Kitap kitap)
    {
        //yeni kitap eklemek ve bilgi yazdırmak için kullanıyoruz
        kitaplar.Add(kitap);
        Console.WriteLine($"{kitap.Baslik} başlıklı kitap başarıyla eklendi.");
    }

    public void TumKitaplariGoruntule()
    {
        //kitapların hepsini ekrana yazdırıyor
        Console.WriteLine("Tüm Kitaplar:");
        foreach (var kitap in kitaplar) //kitaplar listesindeki her bir kitap değeri için "foreach".
        {
            Console.WriteLine($"Başlık: {kitap.Baslik}, Yazar: {kitap.Yazar}, ISBN: {kitap.ISBN}, Kopya Sayısı: {kitap.KopyaSayisi}, Ödünç Alınan Kopya Sayısı: {kitap.OduncAlinanKopyaSayisi}");
        }
    }

    public void KitapAra(string anahtar)
    {
        //kullanıcının girdiği anahtara göre kitapları arıyoruz ve ekrana yazdırıyoruz. Baslik kullanıyoruz veya yazar kullanıyoruz.
        var bulunanKitaplar = kitaplar.Where(kitap => kitap.Baslik.Contains(anahtar) || kitap.Yazar.Contains(anahtar)).ToList();

        if (bulunanKitaplar.Count > 0)
        {
            Console.WriteLine("Arama Sonuçları:");
            foreach (var kitap in bulunanKitaplar)
            {
                Console.WriteLine($"Başlık: {kitap.Baslik}, Yazar: {kitap.Yazar}, ISBN: {kitap.ISBN}, Kopya Sayısı: {kitap.KopyaSayisi}, Ödünç Alınan Kopya Sayısı: {kitap.OduncAlinanKopyaSayisi}");
            }
        }
        else
        {
            Console.WriteLine("Aranan kitap bulunamadı.");
        }
    }

    public void KitapOduncAl(string baslik)
    {
        // kullanıcının girdiği başlıktaki kitabı aratıyoruz
        var kitap = kitaplar.FirstOrDefault(k => k.Baslik == baslik);   //LINQ'te kullanılıyor, listede koşulu sağlayan veya default olan ilk eleman kimse onu getiriyor

        //eğer kitap bulunduysa ve yeterince kopya varsa alınabilecek, devam ediyoruz
        if (kitap != null && kitap.KopyaSayisi > kitap.OduncAlinanKopyaSayisi)
        {
            //alıyoruz
            kitap.OduncAlindi();
        }
        else
        {
            //kitap yoksa, ya da var olduğu halde kopya yetersizse alamıyoruz.
            Console.WriteLine($"{baslik} başlıklı kitap ödünç alınamadı.");
        }
    }

    public void KitapIadeEt(string baslik)
    {
        //kullanıcı girdiği başlığa sahip olan ilk kitabı veya belirlenen default varsa onu iade eder
        var kitap = kitaplar.FirstOrDefault(k => k.Baslik == baslik);

        if (kitap != null && kitap.OduncAlinanKopyaSayisi > 0)  //böyle bir kitap varsa ve elimizde ödünç alınan kopya sayısı 0'dan fazlaysa alta geçeriz
        {
            kitap.OduncAlinanKopyaSayisi--;
            Console.WriteLine($"{baslik} başlıklı kitap iade edildi.");
        }
        else
        {
            Console.WriteLine($"{baslik} başlıklı kitap iade edilemedi.");  //kitap yoksa veya varsa ama biz ödünç almamışsak iade edemeyiz
        }
    }

    public void GecikenKitaplariGoruntule()
    {
        //ödünç alınan kopya sayısı 0'dan fazla ve süresi geçmiş olan kitapları kitaplar listesinde bulur ve yazdırır
        var gecikenKitaplar = kitaplar.Where(kitap => kitap.OduncSuresiGecmis && kitap.OduncAlinanKopyaSayisi > 0).ToList();

        if (gecikenKitaplar.Count > 0)
        {
            Console.WriteLine("Süresi Geçmiş Kitaplar:");
            foreach (var kitap in gecikenKitaplar)  //her birini teker teker yazdırmak için kullanıyoruz
            {
                Console.WriteLine($"Başlık: {kitap.Baslik}, Yazar: {kitap.Yazar}, ISBN: {kitap.ISBN}, Kopya Sayısı: {kitap.KopyaSayisi}, Ödünç Alınan Kopya Sayısı: {kitap.OduncAlinanKopyaSayisi}");
            }
        }
        else
        {
            Console.WriteLine("Süresi geçmiş kitap bulunmamaktadır.");  //yukardaki şartları sağlayan kitap yoksa bu çalışır
        }
    }

}

class Program   //programımız, hazır girdilerle beraber veriyorum ki ekstra iş yükü olmasın.
{
    static void Main()
    {
        Kutuphane kutuphane = new Kutuphane();

        // Örnek kitapları ekliyorum ki başta 1'e basarak hem kütüphane görüntüleyebilelim hem de teker teker kitap eklemek zorunda olmayalım.
        kutuphane.KitapEkle(new Kitap { Baslik = "Homo Ludens", Yazar = "Johan Huizinga", ISBN = "789012", KopyaSayisi = 3 });
        kutuphane.KitapEkle(new Kitap { Baslik = "Press Reset", Yazar = "Jason Schreier", ISBN = "135790", KopyaSayisi = 1 });
        kutuphane.KitapEkle(new Kitap { Baslik = "Blood, Sweat, and Pixels", Yazar = "Jason Schreier", ISBN = "246801", KopyaSayisi = 2 });
        kutuphane.KitapEkle(new Kitap { Baslik = "Çekirge : Oyun, Yaşam ve Ütopya", Yazar = "Bernard Suits", ISBN = "112233", KopyaSayisi = 5 });

        // Üstte girdisini yaptığımız kitapları ekliyoruz.
        kutuphane.KitapEkle(kitap1);
        kutuphane.KitapEkle(kitap2);
        kutuphane.KitapEkle(kitap3);
        kutuphane.KitapEkle(kitap4);

        int secim;
        do
        {
            Console.WriteLine("\n******** KÜTÜPHANE YÖNETİM SİSTEMİ ********");
            Console.WriteLine("1. Tüm Kitapları Görüntüle");
            Console.WriteLine("2. Kitap Ekle");
            Console.WriteLine("3. Kitap Ara");
            Console.WriteLine("4. Kitap Ödünç Al");
            Console.WriteLine("5. Kitap İade Et");
            Console.WriteLine("6. Geciken Kitapları Görüntüle");
            Console.WriteLine("7. Çıkış Yap");
            Console.Write("Lütfen bir seçenek girin (1-7): ");

            if (int.TryParse(Console.ReadLine(), out secim))    // trypase ile bir metni bir tamsayıya dönüştürmeye çalışıyoruz ve girdiğimiz değer zaten numeric'se out ile secim alıyoruz, değilse else'e yönlendiriyor
            {
                switch (secim)  //secim isimli case'ler oluşturuyoruz
                {
                    //1'i tryparse ile okuyarak, secim'e 1 veriyoruz ve yönetim sisteminde de yazdığı gibi TumKitaplariGoruntule fonksiyonu çalışıyor.
                    case 1: 
                        kutuphane.TumKitaplariGoruntule();
                        break;

                    // 2'yi tryparse ile okuyarak, secim'e 2 veriyoruz ve fonksiyonun çalışması için gereken değerleri almaya başlıyoruz.
                    case 2:
                        Console.Write("Eklemek istediğiniz kitabın başlığını girin: ");
                        string baslik = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(baslik))  //hatalı giriş engellemek için tamamen boş veya boşluk karakterini tarıyoruz, değilse girdiyi alıyoruz
                        {
                            Console.WriteLine("Geçersiz başlık girişi.");
                            break;
                        }

                        Console.Write("Yazarını girin: ");
                        string yazar = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(yazar))   //hatalı giriş engellemek için tamamen boş veya boşluk karakterini tarıyoruz, değilse girdiyi alıyoruz
                        {
                            Console.WriteLine("Geçersiz yazar girişi.");
                            break;
                        }

                        Console.Write("ISBN'ini girin: ");
                        string isbn = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(isbn))    //hatalı giriş engellemek için tamamen boş veya boşluk karakterini tarıyoruz, değilse girdiyi alıyoruz
                        {
                            Console.WriteLine("Geçersiz ISBN girişi.");
                            break;
                        }

                        Console.Write("Kopya sayısını girin: ");
                        int kopyaSayisi;
                        if (int.TryParse(Console.ReadLine(), out kopyaSayisi))  //int olarak parse'layabiliyorsak kopya sayısı giriyoruz, değilse alttaki else'e geçiyoruz
                        {
                            // kitap eklemek için gereken verileri yukarıda aldıktan sonra kitabı oluşturuyoruz ve ekliyoruz.
                            Kitap yeniKitap = new Kitap { Baslik = baslik, Yazar = yazar, ISBN = isbn, KopyaSayisi = kopyaSayisi };
                            kutuphane.KitapEkle(yeniKitap);
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz kopya sayısı girişi.");
                        }
                        break;

                    //3'ü tryparse ile okuyarak, secim'e 3 veriyoruz ve "anahtar" olarak adlandırdığımız string değeri ile kütüphane içinde arama yapıyoruz, kitap ara fonksiyonu bunu döndürüyor.
                    case 3: 
                        Console.Write("Aramak istediğiniz kelimeyi girin: ");
                        string anahtar = Console.ReadLine();
                        kutuphane.KitapAra(anahtar);
                        break;

                    //4'ü tryparse ile okuyarak, secim'e 4 veriyoruz ve oduncBaslik ismindeki string ile kütüphanede KitapOduncAl fonksiyonunu çalıştırıyoruz.
                    case 4:
                        Console.Write("Ödünç almak istediğiniz kitabın başlığını girin: ");
                        string oduncAlBaslik = Console.ReadLine();
                        kutuphane.KitapOduncAl(oduncAlBaslik);
                        break;

                    //5'i tryparse ile okuyarak, secim'e 5 veriyoruz ve iadeBaslik ile kütüphanede arama yaparak KitapIadeEt'e bu değeri atıyoruz, iade gerçekleşiyor.
                    case 5:
                        Console.Write("İade etmek istediğiniz kitabın başlığını girin: ");
                        string iadeBaslik = Console.ReadLine();
                        kutuphane.KitapIadeEt(iadeBaslik);
                        break;

                    //6'yı tryparse ile okuyarak, secim'e 6 veriyoruz ve direkt fonksiyon call oluyor.
                    case 6:
                        kutuphane.GecikenKitaplariGoruntule();
                        break;

                    //7'i tryparse ile okuyarak, secim'e 7 veriyoruz ve ekran kapatılıyor
                    case 7:
                        Console.WriteLine("Çıkış yapılıyor...");
                        break;

                    //diğer hiçbir case için geçerli girdi girilmezse, default'umuz bu mesajı veriyor.
                    default:
                        Console.WriteLine("Geçersiz bir seçenek girdiniz. Lütfen tekrar deneyin.");
                        break;
                }
            }
            else  //girilen değer en başta yanlışsa bu else çalışıyor.
            {
                Console.WriteLine("Geçersiz bir sayı girişi yaptınız. Lütfen tekrar deneyin.");
            }

        } while (secim != 7);   //7'i seçmeyene kadar do while loop'u çalışıyor.
    }
}
