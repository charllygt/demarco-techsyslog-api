using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Orders.ValueObjects;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Conventions;

internal sealed class UserIdSerializer : SerializerBase<UserId>
{
    public override UserId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var guid = context.Reader.ReadString();
        return new UserId(Guid.Parse(guid));
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, UserId value) =>
        context.Writer.WriteString(value.Value.ToString());
}

internal sealed class OrderIdSerializer : SerializerBase<OrderId>
{
    public override OrderId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var guid = context.Reader.ReadString();
        return new OrderId(Guid.Parse(guid));
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, OrderId value) =>
        context.Writer.WriteString(value.Value.ToString());
}

internal sealed class NotificationIdSerializer : SerializerBase<NotificationId>
{
    public override NotificationId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var guid = context.Reader.ReadString();
        return new NotificationId(Guid.Parse(guid));
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NotificationId value) =>
        context.Writer.WriteString(value.Value.ToString());
}

internal sealed class EmailSerializer : SerializerBase<Email>
{
    public override Email Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var raw = context.Reader.ReadString();
        return Email.Create(raw).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Email value) =>
        context.Writer.WriteString(value.Value);
}

internal sealed class PasswordHashSerializer : SerializerBase<PasswordHash>
{
    public override PasswordHash Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var raw = context.Reader.ReadString();
        return PasswordHash.Create(raw).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, PasswordHash value) =>
        context.Writer.WriteString(value.Value);
}

internal sealed class CepSerializer : SerializerBase<Cep>
{
    public override Cep Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var raw = context.Reader.ReadString();
        return Cep.Create(raw).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Cep value) =>
        context.Writer.WriteString(value.Value);
}

internal sealed class OrderNumberSerializer : SerializerBase<OrderNumber>
{
    public override OrderNumber Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var raw = context.Reader.ReadString();
        return OrderNumber.Create(raw).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, OrderNumber value) =>
        context.Writer.WriteString(value.Value);
}

internal sealed class MoneySerializer : SerializerBase<Money>
{
    public override Money Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();
        decimal amount = 0;
        string currency = "BRL";
        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var name = context.Reader.ReadName(Utf8NameDecoder.Instance);
            if (name == "amount") amount = (decimal)context.Reader.ReadDecimal128();
            else if (name == "currency") currency = context.Reader.ReadString();
            else context.Reader.SkipValue();
        }
        context.Reader.ReadEndDocument();
        return Money.Create(amount, currency).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Money value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("amount");
        context.Writer.WriteDecimal128((Decimal128)value.Amount);
        context.Writer.WriteName("currency");
        context.Writer.WriteString(value.Currency);
        context.Writer.WriteEndDocument();
    }
}

internal sealed class AddressSerializer : SerializerBase<Address>
{
    public override Address Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();
        string cep = "", street = "", number = "", neighborhood = "", city = "", state = "";
        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var name = context.Reader.ReadName(Utf8NameDecoder.Instance);
            switch (name)
            {
                case "cep": cep = context.Reader.ReadString(); break;
                case "street": street = context.Reader.ReadString(); break;
                case "number": number = context.Reader.ReadString(); break;
                case "neighborhood": neighborhood = context.Reader.ReadString(); break;
                case "city": city = context.Reader.ReadString(); break;
                case "state": state = context.Reader.ReadString(); break;
                default: context.Reader.SkipValue(); break;
            }
        }
        context.Reader.ReadEndDocument();
        var cepVo = Cep.Create(cep).Value;
        return Address.Create(cepVo, street, number, neighborhood, city, state).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Address value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("cep"); context.Writer.WriteString(value.Cep.Value);
        context.Writer.WriteName("street"); context.Writer.WriteString(value.Street);
        context.Writer.WriteName("number"); context.Writer.WriteString(value.Number);
        context.Writer.WriteName("neighborhood"); context.Writer.WriteString(value.Neighborhood);
        context.Writer.WriteName("city"); context.Writer.WriteString(value.City);
        context.Writer.WriteName("state"); context.Writer.WriteString(value.State);
        context.Writer.WriteEndDocument();
    }
}

internal sealed class DeliverySerializer : SerializerBase<Delivery?>
{
    public override Delivery? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }
        var ms = context.Reader.ReadDateTime();
        var utc = BsonUtils.ToDateTimeFromMillisecondsSinceEpoch(ms);
        return Delivery.Create(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).Value;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Delivery? value)
    {
        if (value is null)
        {
            context.Writer.WriteNull();
            return;
        }
        var ms = BsonUtils.ToMillisecondsSinceEpoch(value.DeliveredAt);
        context.Writer.WriteDateTime(ms);
    }
}
