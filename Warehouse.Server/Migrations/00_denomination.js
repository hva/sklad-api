printjson(
    db.products.update(
        { price_opt: { $not: { $type: 1 } } },
        { $mul: { price_opt: 0.0001 } },
        { multi: true }
    )
)

printjson(
    db.products.update(
        { price_rozn: { $not: { $type: 1 } } },
        { $mul: { price_rozn: 0.0001 } },
        { multi: true }
    )
)

printjson(
    res = db.products.update(
        { price_income: { $not: { $type: 1 } } },
        { $mul: { price_income: 0.0001 } },
        { multi: true }
    )
)