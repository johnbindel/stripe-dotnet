﻿using System.Collections.Generic;
using Machine.Specifications;

namespace Stripe.Tests
{
    public class when_updating_an_application_fee_refund
    {
        private static StripeCharge _charge;
        private static StripeApplicationFeeRefund _createdRefund;
        private static StripeApplicationFeeRefund _updatedRefund;

        private static readonly Dictionary<string, string> _metaData = new Dictionary<string, string>
        {
            { "ApprovedBy", "Robert" },
            { "ApprovedReason", "Buckets!" }
        };

        Establish context = () =>
        {
            var customAccount = Cache.GetCustomAccountWithCard();

            var token = new StripeTokenService().Create(test_data.stripe_token_create_options.Valid());

            // create a charge on that custom account with an application fee of 10 cents
            var chargeCreateOptions = test_data.stripe_charge_create_options.ValidToken(token.Id);
            chargeCreateOptions.ApplicationFee = 10;

            _charge = new StripeChargeService().Create(chargeCreateOptions,
                new StripeRequestOptions
                {
                    StripeConnectAccountId = customAccount.Id
                }
            );

            _createdRefund = new StripeApplicationFeeRefundService().CreateAsync(_charge.ApplicationFeeId).Result;
        };

        Because of = () =>
            _updatedRefund = new StripeApplicationFeeRefundService().Update(_charge.ApplicationFeeId, _createdRefund.Id,
                new StripeApplicationFeeRefundUpdateOptions { Metadata = _metaData });

        It should_have_a_refund_object = () =>
            _updatedRefund.ShouldNotBeNull();

        It should_have_the_right_id = () =>
            _updatedRefund.Id.ShouldEqual(_createdRefund.Id);

        It should_have_a_refund_amount_of_ten_cents = () =>
            _updatedRefund.Amount.ShouldEqual(10);

        It should_have_the_right_metadata = () =>
            _updatedRefund.Metadata.ShouldEqual(_metaData);
    }
}
