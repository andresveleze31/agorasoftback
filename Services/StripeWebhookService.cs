// // Services/StripeWebhookService.cs
// using System;
// using System.IO;
// using System.Threading.Tasks;
// using backend.Db;
// using backend.Models;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Options;
// using Stripe;

// namespace backend.Services
// {
//     public interface IStripeWebhookService
//     {
//         Task HandleWebhookEvent(string json, string stripeSignature);
//     }

//     public class StripeWebhookService : IStripeWebhookService
//     {
//         private readonly AppDbContext _dbContext;
//         private readonly string _webhookSecret;

//         public StripeWebhookService(AppDbContext dbContext, IOptions<StripeModel> stripeConfig)
//         {
//             _dbContext = dbContext;
//             _webhookSecret = stripeConfig.Value.WebhookSecret; // Necesitarás agregar esto a tu StripeModel
//         }

//         public async Task HandleWebhookEvent(string json, string stripeSignature)
//         {
//             try
//             {
//                 var stripeEvent = EventUtility.ConstructEvent(
//                     json, stripeSignature, _webhookSecret);

//                 switch (stripeEvent.Type)
//                 {
//                     case Events.CustomerSubscriptionCreated:
//                     case Events.CustomerSubscriptionUpdated:
//                         await HandleSubscriptionUpdated(stripeEvent);
//                         break;
                    
//                     case Events.CustomerSubscriptionDeleted:
//                         await HandleSubscriptionDeleted(stripeEvent);
//                         break;
                    
//                     case Events.InvoicePaymentSucceeded:
//                         await HandleInvoicePaymentSucceeded(stripeEvent);
//                         break;
                    
//                     case Events.InvoicePaymentFailed:
//                         await HandleInvoicePaymentFailed(stripeEvent);
//                         break;
//                 }
//             }
//             catch (StripeException ex)
//             {
//                 throw new ApplicationException("Error processing webhook", ex);
//             }
//         }

//         private async Task HandleSubscriptionUpdated(Event stripeEvent)
//         {
//             var subscription = stripeEvent.Data.Object as Stripe.Subscription;
//             if (subscription == null) return;

//             await UpdateSubscriptionInDatabase(subscription);
//         }

//         private async Task HandleSubscriptionDeleted(Event stripeEvent)
//         {
//             var subscription = stripeEvent.Data.Object as Stripe.Subscription;
//             if (subscription == null) return;

//             var dbSubscription = await _dbContext.Subscriptions
//                 .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

//             if (dbSubscription != null)
//             {
//                 dbSubscription.Status = "canceled";
//                 dbSubscription.CanceledAt = DateTime.UtcNow;
//                 dbSubscription.UpdatedAt = DateTime.UtcNow;

//                 await _dbContext.SaveChangesAsync();
//             }
//         }

//         private async Task HandleInvoicePaymentSucceeded(Event stripeEvent)
//         {
//             var invoice = stripeEvent.Data.Object as Stripe.Invoice;
//             if (invoice?.SubscriptionId == null) return;

//             // Obtener la suscripción de Stripe para actualizar el periodo
//             var subscriptionService = new SubscriptionService();
//             var subscription = await subscriptionService.GetAsync(invoice.SubscriptionId);

//             await UpdateSubscriptionInDatabase(subscription);
//         }

//         private async Task HandleInvoicePaymentFailed(Event stripeEvent)
//         {
//             var invoice = stripeEvent.Data.Object as Stripe.Invoice;
//             if (invoice?.SubscriptionId == null) return;

//             var dbSubscription = await _dbContext.Subscriptions
//                 .FirstOrDefaultAsync(s => s.StripeSubscriptionId == invoice.SubscriptionId);

//             if (dbSubscription != null)
//             {
//                 // Marcar como past_due o incomplete según sea necesario
//                 dbSubscription.Status = "past_due";
//                 dbSubscription.UpdatedAt = DateTime.UtcNow;

//                 await _dbContext.SaveChangesAsync();
//             }
//         }

//         private async Task UpdateSubscriptionInDatabase(Stripe.Subscription subscription)
//         {
//             var dbSubscription = await _dbContext.Subscriptions
//                 .Include(s => s.Organization)
//                 .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

//             if (dbSubscription == null)
//             {
//                 // Buscar organización por Stripe Customer ID
//                 var organization = await _dbContext.Organizations
//                     .FirstOrDefaultAsync(o => o.StripeCustomerId == subscription.CustomerId);

//                 if (organization != null)
//                 {
//                     dbSubscription = new Subscription
//                     {
//                         StripeSubscriptionId = subscription.Id,
//                         StripeCustomerId = subscription.CustomerId,
//                         PriceId = subscription.Items.Data[0].Price.Id,
//                         Status = subscription.Status,
//                         CurrentPeriodStart = subscription.CurrentPeriodStart,
//                         CurrentPeriodEnd = subscription.CurrentPeriodEnd,
//                         CanceledAt = subscription.CanceledAt,
//                         OrganizationId = organization.Id,
//                         CreatedAt = DateTime.UtcNow
//                     };

//                     _dbContext.Subscriptions.Add(dbSubscription);
//                 }
//             }
//             else
//             {
//                 dbSubscription.Status = subscription.Status;
//                 dbSubscription.CurrentPeriodStart = subscription.CurrentPeriodStart;
//                 dbSubscription.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
//                 dbSubscription.CanceledAt = subscription.CanceledAt;
//                 dbSubscription.UpdatedAt = DateTime.UtcNow;
//             }

//             await _dbContext.SaveChangesAsync();
//         }
//     }
// }