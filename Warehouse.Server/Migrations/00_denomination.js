function convert(x, key) {
	var price = x[key]
	if (price instanceof NumberLong || price instanceof NumberInt) {
		var price2 = Math.ceil(price / 1000) / 10
		x[key] = price2
		print([key, price, '-> ' + price2].join('\t'))
		return 1
	}
	return 0
}

// demoninates last 4 digits of price fields with ceiling up to 10 cents
// example: 7614071 -> 761.5
function update(x) {
	var isModified =
		convert(x, 'price_opt') |
		convert(x, 'price_rozn') |
		convert(x, 'price_income')
	if (isModified) {
		var res = db.products.save(x)
		printjson(res)
		print('\n')
	}
}

db.products.find().forEach(update)
