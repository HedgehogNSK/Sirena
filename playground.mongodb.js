
use('playground');
// db.users.drop();
// db.users.insertMany([{
//   "_id": ObjectId( "65f0dad2db766d26da3b088d"
//   ),
//   "ownerid": NumberLong( "402690255"),
//   "title": "Test2",
//   "count": NumberLong( "0"),
//   "requests": [
//     {
//       "user_id": NumberLong( "402690236"),
//       "message": "Дай права"
//     }
//   ],
//   "listener": [
//     NumberLong( "402690236"),
//   ]
// },
// {
//   "_id": ObjectId( "65fc0f83c34e3b20edac4642"),
//   "ownerid": NumberLong( "402690236"),
//   "title": "Check",
//   "count": NumberLong( "0"),
//   "listener": [
//     NumberLong( "6443061333"),
//   ],
//   "responsible": [
//     NumberLong( "402690236"),
//   ],
//   "requests": [],
//   "last_call": {
//     "caller": NumberLong( "402690236"),
//     "date": {
//       "$date": NumberLong( "1711749420803"),
//     }
//   }
// },
// {
//   "_id": ObjectId( "65fc94153273fc2ab6ff895f"),
//   "ownerid": NumberLong( "402690236"),
//   "title": "Ждём ТоИ 3",
//   "count": NumberLong( "0"),
//   "listener": [
//     NumberLong( "697730978"),
//   ],
//   "responsible": [],
//   "requests": [],
//   "last_call": {
//     "caller": NumberLong( "402690236"),
//     "date": {
//       "$date": NumberLong( "1711749390536"),
//     }
//   }
// },
// {
//   "_id": ObjectId( "65fc94593273fc2ab6ff8960"),
//   "ownerid": NumberLong( "6443061333"),
//   "title": "Ждём ТоИ 3",
//   "count": NumberLong( "0"),
//   "listener": [
//     NumberLong( "402690236"),
//   ],
//   "responsible": [],
//   "requests": [],
//   "last_call": {
//     "caller": NumberLong( "6443061333"),
//     "date": {
//       "$date": NumberLong( "1711052809476"),
//     }
//   }
// },
// {
//   "_id": ObjectId( "65ffce29484f23ec57f116a0"),
//   "ownerid": NumberLong( "697730978"),
//   "title": "Satonyx",
//   "count": NumberLong( "0"),
//   "listener": [],
//   "responsible": [],
//   "requests": []
// }]);

db.sirens.drop();
db.sirens.insertMany([{
  "_id": ObjectId( "65f0dad2db766d26da3b088d"),
  "ownerid": NumberLong( "402690255"),
  "title": "Test2",
  "count": NumberLong( "0"),
  "requests": [
    {
      "user_id": NumberLong( "402690236"),
      "message": "Дай права"
    }
  ],
  "listener": [
    NumberLong( "402690236"),
  ]
},
{
  "_id": ObjectId( "65fc0f83c34e3b20edac4642"),
  "ownerid": NumberLong( "402690236"),
  "title": "Check",
  "count": NumberLong( "0"),
  "listener": [
    NumberLong( "6443061333"),
  ],
  "responsible": [
    NumberLong( "6443061333"),
  ],
  "requests": [],
  "last_call": {
    "caller": NumberLong( "402690236"),
    "date": {
      "$date": NumberLong( "1711749420803"),
    }
  }
},
{
  "_id": ObjectId( "65fc94153273fc2ab6ff895f"),
  "ownerid": NumberLong( "402690236"),
  "title": "Ждём ТоИ 3",
  "count": NumberLong( "0"),
  "listener": [
    NumberLong( "697730978"),
  ],
  "responsible": [],
  "requests": [],
  "last_call": {
    "caller": NumberLong( "402690236"),
    "date": {
      "$date": NumberLong( "1711749390536"),
    }
  }
},
{
  "_id": ObjectId( "65fc94593273fc2ab6ff8960"),
  "ownerid": NumberLong( "6443061333"),
  "title": "Ждём ТоИ 3",
  "count": NumberLong( "0"),
  "listener": [
    NumberLong( "402690236"),
    NumberLong( "00000000")
  ],
  "responsible": [
    NumberLong( "402690236")
    ],
  "requests": [],
  "last_call": {
    "caller": NumberLong( "6443061333"),
    "date": {
      "$date": NumberLong( "1711052809476"),
    }
  }
},
{
  "_id": ObjectId( "65ffce29484f23ec57f116a0"),
  "ownerid": NumberLong( "697730978"),
  "title": "Satonyx",
  "count": NumberLong( "0"),
  "listener": [],
  "responsible": [],
  "requests": []
}]);

var userId = 402690236; // Ваш ID пользователя
db.sirens.aggregate([{
        "$match": {
            "$or": [
                {"ownerid": userId}, 
                {"listener": userId},
                {"responsible": userId}
            ]
        }
    }
    , {"$group": {
            "_id": true,
            "_elements": {
                "$push": {
                    "SirenasCount": {
                        "$cond": {
                            "if": {
                                "$eq": ["$ownerid", userId]
                            },
                            "then": 1,
                            "else": 0
                        }
                    },
                    "Subscriptions": {
                        "$cond": {
                            "if": {
                                "$and": [
                                    {"$ne": ["$listener", null]}, 
                                    {"$or": [
                                        {"$eq": ["$listener._t", "Array"]},
                                        {"$and": [{"$isArray": "$listener"}]}
                                        ]
                                    }, 
                                    {"$in": [userId, "$listener"]}
                                ]
                            },
                            "then": 1,
                            "else": 0
                        }
                    },
                    "Responsible": {
                        "$cond": {
                            "if": {
                                "$and": [
                                    {"$or": [
                                        {"$isArray": "$responsible"}, 
                                        {"$and": [
                                            {"$isArray": "$responsible._t"},
                                            {"$in": ["Array", "$responsible._t"]}
                                        ]}
                                        ]}, 
                                    {"$ne": ["$responsible", null]},
                                    {"$in": [userId, "$responsible"]}
                                ]
                            },
                            "then": 1,
                            "else": 0
                        }
                    }
                }
            }
        }
    }
]);

//Working solution for get user statistics
// db.sirens.aggregate([
//   {
//     "$match": {
//       "$or": [
//         { "ownerid": userId },
//         { "listener": userId },
//         { "responsible": userId }
//       ]
//     }
//   },
//   {
//     "$group": {
//       "_id": null,
//       "owner": { "$sum": { "$cond": [{ "$eq": ["$ownerid", userId ] }, 1, 0] }},
//       "responsible": { "$sum": { "$cond": [{ "$and": [
//                  { "$ne": ["$responsible", null] },
//                 { "$isArray": "$responsible" }, 
//                 { "$in": [userId, "$responsible"] }
//               ] }, 1,0 ] }}
//       ,"listener": { "$sum": { "$cond": [{ "$and": [
//                  { "$ne": ["$listener", null] },
//                 { "$isArray": "$listener" }, 
//                 { "$in": [userId, "$listener"] }
//               ] }, 1,0 ] }}   
//     }
//   }
// ]);
// db.users.find().forEach(function(user) {
//     let array = user.listener.map(function(id) { return ObjectId(id); } );
//     //console.log(user.listener);
//     if(array.length==0) return;
    
//     let x =db.sirens.updateMany(
//         { "_id": { $in: array } },
//         { $addToSet: { "listener": user._id } }
//     );
//     //console.log(x);
    
// });

// db.sirens.find().forEach(function(siren) {
//     let sirena_id = siren._id;
//     db.users.find({ "requests.sirena_id": sirena_id }).forEach(function(user) {
//         let matchingRequests = user.requests.filter(function(request) {
//             console.log(sirena_id +  "  " + request.sirena_id +" "+request.sirena_id.equals(sirena_id));
//             return request.sirena_id.equals(sirena_id);
//         }).map(function(request) {
//             delete request.sirena_id;
//             return request;
//         });
//         if(matchingRequests.length ==0)return;
//         console.log(matchingRequests);
//         let x = db.sirens.updateOne(
//             { "_id": sirena_id },
//             { $addToSet: { "requests": { $each: matchingRequests } } }
//         );
//     console.log(x);

//     });
// });


//Unset array
//db.users.updateMany({}, { $unset: { "listener": "" } });
//db.users.updateMany({}, { $unset: { "requests": "" } });
// let userId = 402690236;
// let IndexToRemove = 1;

// db.users.updateOne({ "_id": userId }, { $set: { "owner": { $arrayElemAt: ["$owner", IndexToRemove] } } })
// let x = db.users.findOneAndUpdate({ "_id": userId },
//     [{
//         $set: {
//             "owner": {
//                 $concatArrays: [
//                     { $slice: ["$owner", 0, IndexToRemove] },
//                     { $slice: ["$owner", { $add: [IndexToRemove, 1] }, { $size: "$owner" }] }
//                 ]
//             }
//         }
//     }],
//     { projection: { "_id": { $arrayElemAt: ["$owner", 1] } } }
// );

// x = db.sirens.findOneAndDelete({"_id": x._id});


//db.users.find();


// /* global use, db */
// // MongoDB Playground
// // To disable this template go to Settings | MongoDB | Use Default Template For Playground.
// // Make sure you are connected to enable completions and to be able to run a playground.
// // Use Ctrl+Space inside a snippet or a string literal to trigger completions.
// // The result of the last command run in a playground is shown on the results panel.
// // By default the first 20 documents will be returned with a cursor.
// // Use 'console.log()' to print to the debug output.
// // For more documentation on playgrounds please refer to
// // https://www.mongodb.com/docs/mongodb-vscode/playgrounds/

// // Select the database to use.
// use('mongodbVSCodePlaygroundDB');

// // Insert a few documents into the sales collection.
// db.getCollection('sales').insertMany([
//   { 'item': 'abc', 'price': 10, 'quantity': 2, 'date': new Date('2014-03-01T08:00:00Z') },
//   { 'item': 'jkl', 'price': 20, 'quantity': 1, 'date': new Date('2014-03-01T09:00:00Z') },
//   { 'item': 'xyz', 'price': 5, 'quantity': 10, 'date': new Date('2014-03-15T09:00:00Z') },
//   { 'item': 'xyz', 'price': 5, 'quantity': 20, 'date': new Date('2014-04-04T11:21:39.736Z') },
//   { 'item': 'abc', 'price': 10, 'quantity': 10, 'date': new Date('2014-04-04T21:23:13.331Z') },
//   { 'item': 'def', 'price': 7.5, 'quantity': 5, 'date': new Date('2015-06-04T05:08:13Z') },
//   { 'item': 'def', 'price': 7.5, 'quantity': 10, 'date': new Date('2015-09-10T08:43:00Z') },
//   { 'item': 'abc', 'price': 10, 'quantity': 5, 'date': new Date('2016-02-06T20:20:13Z') },
// ]);

// // Run a find command to view items sold on April 4th, 2014.
// const salesOnApril4th = db.getCollection('sales').find({
//   date: { $gte: new Date('2014-04-04'), $lt: new Date('2014-04-05') }
// }).count();

// // Print a message to the output window.
// console.log(`${salesOnApril4th} sales occurred in 2014.`);

// // Here we run an aggregation and open a cursor to the results.
// // Use '.toArray()' to exhaust the cursor to return the whole result set.
// // You can use '.hasNext()/.next()' to iterate through the cursor page by page.
// db.getCollection('sales').aggregate([
//   // Find all of the sales that occurred in 2014.
//   { $match: { date: { $gte: new Date('2014-01-01'), $lt: new Date('2015-01-01') } } },
//   // Group the total sales for each product.
//   { $group: { _id: '$item', totalSaleAmount: { $sum: { $multiply: [ '$price', '$quantity' ] } } } }
// ]);