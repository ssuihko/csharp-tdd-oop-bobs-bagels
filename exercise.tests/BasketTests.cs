using NUnit.Framework;
using exercise.main;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NuGet.Frameworks;

namespace exercise.tests;

[TestFixture]
public class BasketTests
{
    private Basket testBasket;
    private Dictionary<string, Item> inv;

    [SetUp]
    public void Setup()
    {
        IInventory wholeInventory = new WholeInventory();
        IInventory bagelInventory = new BagelInventory();
        IInventory coffeeInventory = new CoffeeInventory();
        IInventory fillingsInventory = new FillingInventory();
        testBasket = new Basket(wholeInventory);  // default
        inv = wholeInventory.getInventory();
    }

    [Test]
    public void AddItem()
    {
        Item item = inv["BGLO"];

        Item res = testBasket.AddItem(item);

        Assert.That(res.SKU, Is.EqualTo("BGLO"));

        Bagel item2 = new Bagel("AAAA", "Fake", 0.0F, "B");

        Item res2 = testBasket.AddItem(item2);

        Assert.That(res2.SKU, Is.EqualTo("none"));

    }

    [Test]
    public void RemoveItem()
    {
        Item item = inv["BGLO"];
        Item item2 = inv["BGLP"];
        Bagel itemFake = new Bagel("AAAA", "Fake", 0.0F, "B");

        testBasket.AddItem(item);
        testBasket.AddItem(item2);

        bool res1 = testBasket.RemoveItem(item);
        Assert.IsTrue(res1);

        bool res2 = testBasket.RemoveItem(itemFake);
        Assert.IsFalse(res2);

    }

    [Test]
    public void CapacityExceededMessage()
    {
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLP"]);
        testBasket.AddItem(inv["COFB"]);
        testBasket.AddItem(inv["BGLS"]);
        testBasket.AddItem(inv["COFW"]);

        Item res = testBasket.AddItem(inv["BGLE"]);

        var outputLines = stringWriter.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That("Basket size exceeded!", Is.EqualTo(outputLines[0]));

        Assert.That(res.SKU, Is.EqualTo("none"));
  
    }

    [Test]
    public void ChangeCapacity()
    {
        
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        testBasket.ChangeCapacity(3);

        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLP"]);
        testBasket.AddItem(inv["COFB"]);

        Item res = testBasket.AddItem(inv["BGLE"]);

        var outputLines = stringWriter.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That("Basket size exceeded!", Is.EqualTo(outputLines[0]));

        Assert.That(res.SKU, Is.EqualTo("none"));
        
    }

    [Test]
    public void NotRemovable()
    {
       
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        Item t1 = testBasket.AddItem(inv["BGLO"]);
        Item t2 = testBasket.AddItem(inv["BGLO"]);

        bool res = testBasket.RemoveItem(inv["BGLE"]);

        var outputLines = stringWriter.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That("Item not in basket!", Is.EqualTo(outputLines[0]));

        Assert.IsFalse(res);
        

    }

    [Test]
    public void TotalCostReceived()
    {
        
        float totalInit = testBasket.TotalCost();

        Assert.That(totalInit, Is.EqualTo(0F));

        Item t1 = testBasket.AddItem(inv["BGLO"]);
        Item t2 = testBasket.AddItem(inv["BGLE"]);

        float total = testBasket.TotalCost();

        Assert.That(total, Is.EqualTo(0.98F));
        
    }

    [Test]
    public void ItemPriceRetreivable()
    {
        
        float itemPrice1 = testBasket.GetItemPrice(inv["BGLO"]);
        float itemPrice2 = testBasket.GetItemPrice(inv["BGLP"]);

        Assert.That(itemPrice1, Is.EqualTo(0.49F));
        Assert.That(itemPrice2, Is.EqualTo(0.39F));

        Bagel item2 = new Bagel("AAAA", "Fake", 0.0F, "B");

        float itemPrice3 = testBasket.GetItemPrice(item2);
        Assert.That(itemPrice3, Is.EqualTo(0F));
        
    }

    [Test]
    public void FillingPriceRetreivable()
    {
        
        float itemPrice1 = testBasket.GetItemPrice(inv["FILX"]);
        float itemPrice2 = testBasket.GetItemPrice(inv["FILS"]);

        Assert.That(itemPrice1, Is.EqualTo(0.12F));
        Assert.That(itemPrice2, Is.EqualTo(0.12F));
        
    }

    [Test]
    public void AddFillings()
    {
        
        Bagel it1 = (Bagel)testBasket.AddItem(inv["BGLE"]);
        Bagel it2 = (Bagel)testBasket.AddItem(inv["BGLE"]);

        Filling f1 = (Filling)inv["FILE"];
        Filling f2 = (Filling)inv["FILH"];
        Filling f3 = (Filling)inv["FILB"];

        Assert.That(it1.SKU, Is.EqualTo("BGLE"));
        Assert.That(it2.SKU, Is.EqualTo("BGLE"));


        testBasket.AddFilling(it1.ID, f1);
        testBasket.AddFilling(it2.ID, f2);
        testBasket.AddFilling(it2.ID, f3);

        List<Item> fillings = it1.ListFillings();
        Assert.That(fillings[0].SKU, Is.EqualTo("FILE"));

        List<Item> fillings2 = it2.ListFillings();
        Assert.That(fillings2[0].SKU, Is.EqualTo("FILH"));
        Assert.That(fillings2[1].SKU, Is.EqualTo("FILB"));
        
    }

    [Test]
    public void OnlyValidOrders()
    {
        Bagel item2 = new Bagel("AAAA", "Fake", 0.0F, "B");
        Filling item3 = new Filling("AAAA", "Fake", 0.0F, "B");

        Item it1 = testBasket.AddItem(item2);
        Assert.That(it1.SKU, Is.EqualTo("none"));

        Bagel it2 = (Bagel)testBasket.AddItem(inv["BGLE"]);
        testBasket.AddFilling(it2.ID, item3);

        List<Item> fillings = it2.ListFillings();
        Assert.AreEqual(fillings.Count, 0);
        

    }

    [Test]
    public void DiscountBagelBundle6()
    {
        
        testBasket.ChangeCapacity(15);

        Item i1 = testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);

        testBasket.BundleOrder("b6", i1, i1);

        float totalInit = testBasket.TotalCost();
        Assert.That(totalInit, Is.EqualTo(2.49F));
        
    }

    [Test]
    public void DiscountBagelBundle12()
    {
        
        testBasket.ChangeCapacity(15);

        Item i1 = testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["BGLE"]);

        testBasket.BundleOrder("b12", i1, i1);

        float totalInit2 = testBasket.TotalCost();
        Assert.That(totalInit2, Is.EqualTo(3.99F));
        
    }

    [Test]
    public void DiscountCoffeeAndBagel()
    {
        
        Item i1 = testBasket.AddItem(inv["BGLE"]);
        Item i2 = testBasket.AddItem(inv["COFB"]);

        testBasket.BundleOrder("bac", i1, i2);

        float totalInit2 = testBasket.TotalCost();
        Assert.That(totalInit2, Is.EqualTo(1.25F));
        
    }

    [Test]
    public void ReceitPrinted()
    {        
        testBasket.ChangeCapacity(20);
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        
        testBasket.AddItem(inv["BGLE"]);
        testBasket.AddItem(inv["COFB"]);
        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLO"]);
        testBasket.AddItem(inv["BGLO"]);

        testBasket.PrintReceipt();

        var outputLines = stringWriter.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.That("     ~~~ Bob's Bagels ~~~", Is.EqualTo(outputLines[0]));

        DateTime enteredDate = DateTime.Parse(outputLines[1]);
        DateTime now = DateTime.Now;

        Assert.That(enteredDate, Is.InstanceOf(now.GetType()));  // DateTime
        Assert.That("------------------------------", Is.EqualTo(outputLines[2]));
        Assert.That("Everything Bagel      1  �0,49", Is.EqualTo(outputLines[3]));
        Assert.That("Black Coffee          1  �0,99", Is.EqualTo(outputLines[4]));
        Assert.That("Onion Bagel           6  �2,49", Is.EqualTo(outputLines[5]));

        Assert.That("------------------------------", Is.EqualTo(outputLines[6]));
        Assert.That("Total                    �4,42", Is.EqualTo(outputLines[7]));
        Assert.That("           Thank you", Is.EqualTo(outputLines[8]));
        Assert.That("        for your order!", Is.EqualTo(outputLines[9]));
        
    }

}