using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Rappd.Data;
using Rappd.Data.Bson;
using Rappd.Data.Bson.Sample.Abstractions;

[assembly: ImplementsFrom<Program>]

BsonInterfaceRegistrator.RegisterInterfaces();

BsonInterfaceRegistrator.RegisterInterfaceMap<IBase>(cm =>
{
    cm.MapIdProperty(nameof(IBase.Identifier));
});

var person = Implementations.CreateIPerson("Max", "Mustermann", Implementations.CreateISub("Name", "Id"));

BsonDocument personDocument = new BsonDocument();
using (var writer = new BsonDocumentWriter(personDocument))
    BsonSerializer.Serialize(writer, person);

var deserializedPerson = BsonSerializer.Deserialize<IPerson>(personDocument);

IBase sub = Implementations.CreateISub(
    "123",
    "subbb"
);
BsonDocument subDocument = new BsonDocument();
using (var writer = new BsonDocumentWriter(subDocument))
    BsonSerializer.Serialize(writer, sub);

Console.ReadLine();