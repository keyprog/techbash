using BuildingBricks.Shipping.Models;
using BuildingBricks.Shipping.Requests;

namespace BuildingBricks.Shipping;

public class ShippingServices : ServicesBase
{

	public ShippingServices(ConfigServices configServices) : base(configServices) { }

	public async Task UpdateShipmentStatusAsync(
		int shipmentId,
		UpdateShipmentStatusRequest updateShipmentStatusRequest)
	{

		using ShippingContext shippingContext = new(_configServices);

		// Retrieve the shipment to be updated
		Shipment? shipment = await shippingContext.Shipments
			.Include(x => x.CustomerPurchase)
			.FirstOrDefaultAsync(x => x.ShipmentId == shipmentId)
			?? throw new ArgumentOutOfRangeException(nameof(shipmentId));

		// Validate the specified shipment status
		ShipmentStatus? shipmentStatus = await shippingContext.ShipmentStatuses.FirstOrDefaultAsync(x => x.ShipmentStatusId == updateShipmentStatusRequest.ShipmentStatusId)
			?? throw new ArgumentOutOfRangeException(nameof(updateShipmentStatusRequest), "Invalid shipping status specified.");

		// Validate the specified shipping carrier
		ShippingCarrier? carrier = null;
		if (updateShipmentStatusRequest.CarrierId is not null)
		{
			carrier = await shippingContext.ShippingCarriers.FirstOrDefaultAsync(x => x.ShippingCarrierId == updateShipmentStatusRequest.CarrierId)
				?? throw new ArgumentOutOfRangeException(nameof(updateShipmentStatusRequest), "Invalid shipping carrier specified.");
		}

		// Update the shipment record
		shipment.ShipmentStatusId = shipmentStatus.ShipmentStatusId;
		shipment.ShippingCarrierId = carrier?.ShippingCarrierId ?? null;
		shipment.TrackingNumber = updateShipmentStatusRequest.TrackingNumber;
		await shippingContext.SaveChangesAsync();

	}

}