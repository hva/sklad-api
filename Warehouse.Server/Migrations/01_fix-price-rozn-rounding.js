function convert(x) {
	var _priceOpt = x.price_opt
	var _k = x.k
	var rozn = _priceOpt * _k / 1000 * 1.2
	if (x.is_sheet) {
		var _l = x.length
		rozn *= _l
	}
	priceRozn2 = Math.ceil(rozn * 100) / 100
	if (priceRozn2 != x.price_rozn) {
		print([x.name + ' ' + x.size, x.price_rozn, '->', priceRozn2].join('\t'))
		x.price_rozn = priceRozn2
		return 1
	}
	return 0
}

// calculates rozn price rounded to 1 cent and updates in db if necessary
function update(x) {
	if (convert(x)) {
		var res = db.products.save(x)
		printjson(res)
		print('\n')
	}
}

db.products.find().forEach(update)
